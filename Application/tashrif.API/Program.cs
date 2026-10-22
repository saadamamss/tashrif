using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using tashrif.API.Hubs;
using tashrif.API.Services;
using tashrif.Email;

// Serilog structured logging (P1 #4): JSON to console (prod log drains) +
// human-readable daily-rotating file (30-day retention). Logger created BEFORE
// builder.Build() so startup failures log instead of dying silently.
// File sink skipped on Railway (ephemeral disk, detected via RAILWAY_ENVIRONMENT).
// Levels/sinks are code-defined here — no Serilog:* config keys are read.
var serilogConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext();
serilogConfig = serilogConfig.WriteTo.Console(new Serilog.Formatting.Compact.CompactJsonFormatter());
// Railway mounts an ephemeral FS — a file sink there writes to storage that dies with
// the container (console drain is the only durable sink), so skip it.
// RAILWAY_ENVIRONMENT is Railway's standard env marker ("production"/inet). Locally it's unset → file sink on.
var isRailwayEnv = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT"));
if (!isRailwayEnv)
{
    serilogConfig = serilogConfig.WriteTo.File(
        Path.Combine("logs", "tashrif-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");
}
Log.Logger = serilogConfig.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Hosting platforms (Railway, etc.) assign a dynamic port via $PORT.
// When present it wins over the appsettings Kestrel endpoint (:5001 local dev).
var railwayPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(railwayPort) && int.TryParse(railwayPort, out var port))
{
    builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(port));
}

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("X-CSRF-Token");
    });
});

// Add rate limiting on auth endpoints (per IP)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, _) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new { message = "تم تجاوز حد المحاولات. الرجاء المحاولة لاحقاً." });
    };

    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.AddPolicy("register", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 3,
                QueueLimit = 0,
                Window = TimeSpan.FromHours(1)
            }));
});

// Add JWT Authentication
static string GetRequiredConfig(IConfiguration configuration, string key) =>
    configuration[key] is { Length: > 0 } value
        ? value
        : throw new InvalidOperationException($"Missing required configuration: {key}");

var jwtKey = GetRequiredConfig(builder.Configuration, "Jwt:Key");
var jwtRefreshKey = GetRequiredConfig(builder.Configuration, "Jwt:RefreshKey");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "tashrif-api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "tashrif-client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Individual", policy => policy.RequireRole("individual"));
    options.AddPolicy("Entity", policy => policy.RequireRole("entity"));
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
});

// Health checks (P1 #4): DbContext canary — selects 1 at the provider level so it
// runs even before migrations. (Response shape documented at the /healthz mapping below.)
builder.Services.AddHealthChecks()
    .AddDbContextCheck<tashrif.Context.tashrifDBContext>("database");

// Add DbContext
builder.Services.AddtashrifContextDependencies(builder.Configuration);

// Add Repositories
builder.Services.AddtashrifRepositoryDependencies();

// Add Services
builder.Services.AddtashrifCoreDependencies();

// Add Identity
builder.Services.AddtashrifIdentityDependencies();

// Add Email
builder.Services.AddtashrifEmailDependencies();

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumReceiveMessageSize = 32 * 1024;
});

// Add NotificationHubService
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();

// Add Background Task Queue
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

// Add hosted services
builder.Services.AddHostedService<JobExpiryService>();
builder.Services.AddHostedService<ContractExpiryService>();

var app = builder.Build();

// Configure the HTTP request pipeline
// One structured line per request — logged at the very front so EVERY terminal
// outcome (auth 401, middleware 400, exceptions → 500, uploads) is captured.
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, _, ex) =>
        ex != null || httpContext.Response.StatusCode >= 500
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode >= 400
                ? LogEventLevel.Warning
                : LogEventLevel.Information;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseMiddleware<tashrif.API.Middleware.ExceptionMiddleware>();
app.UseMiddleware<tashrif.API.Middleware.CsrfMiddleware>();
app.UseMiddleware<tashrif.API.Middleware.CookieToHeaderMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health endpoint (P1 #4): unauthenticated liveness/readiness probe for Railway /
// uptime monitors. 200 when the DB answers, 503 otherwise. Body is the DEFAULT
// health-check writer's plain text ("Healthy" / "Unhealthy: …") — status-code
// monitors are the consumer; no JSON writer is wired on purpose.
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
});

app.MapHub<NotificationHub>("/hubs/notifications").RequireAuthorization();

// Dev-only admin seed (D2): idempotent, no secrets in migrations.
// Wrapped in try/catch so startup doesn't fail if the DB isn't reachable/migrated yet.

// Host lifetime wrapped so Log.CloseAndFlushAsync() (finally) reliably flushes
// Serilog's buffered/queued file entries on graceful shutdown OR host failure.
try
{
if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrif.Context.tashrifDBContext>();
        if (!db.users.Any(u => u.type == "admin" && !u.IsDeleted))
        {
            db.users.Add(new tashrif.Data.users
            {
                national_id = "0000000000",
                password_hash = BCrypt.Net.BCrypt.HashPassword("admin"),
                name = "مدير النظام",
                email = "admin@tashrif.sa",
                phone = "0500000000",
                type = "admin",
                gender = "male",
                nationality = "SA",
                avatar_url = "",
                is_email_verified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
            await db.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Admin seed skipped (database not reachable?)");
    }
}

await app.RunAsync();
}
finally
{
// Serilog tail-flush: buffered/queued file entries still in memory get written
// before the process exits — without this 2-liner, crash-buffered entries are lost.
await Log.CloseAndFlushAsync();
}
