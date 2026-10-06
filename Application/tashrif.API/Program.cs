using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using tashrif.API.Hubs;
using tashrif.API.Services;
using tashrif.Email;

var builder = WebApplication.CreateBuilder(args);

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
app.MapHub<NotificationHub>("/hubs/notifications").RequireAuthorization();

// Dev-only admin seed (D2): idempotent, no secrets in migrations.
// Wrapped in try/catch so startup doesn't fail if the DB isn't reachable/migrated yet.
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

app.Run();
