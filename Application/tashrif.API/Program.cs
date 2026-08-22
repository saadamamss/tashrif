using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

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
              .AllowCredentials();
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
app.UseMiddleware<tashrif.API.Middleware.CookieToHeaderMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
