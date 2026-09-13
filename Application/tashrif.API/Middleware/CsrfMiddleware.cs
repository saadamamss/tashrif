using System.Security.Cryptography;
using System.Text;

namespace tashrif.API.Middleware;

public class CsrfMiddleware
{
    private readonly RequestDelegate _next;
    private const string CookieName = "csrf_token";
    private const string HeaderName = "X-CSRF-Token";
    private const int TokenLength = 32;

    public CsrfMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Set CSRF cookie if not present (for GET requests that initialize the session)
        if (!context.Request.Cookies.ContainsKey(CookieName))
        {
            var token = GenerateToken();
            context.Response.Cookies.Append(CookieName, token, new CookieOptions
            {
                HttpOnly = false, // Frontend needs to read this
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                MaxAge = TimeSpan.FromHours(1),
            });
        }

        // Validate CSRF on state-changing methods (POST, PUT, DELETE, PATCH)
        if (HttpMethods.IsPost(context.Request.Method) ||
            HttpMethods.IsPut(context.Request.Method) ||
            HttpMethods.IsDelete(context.Request.Method) ||
            HttpMethods.IsPatch(context.Request.Method))
        {
            // Skip CSRF for auth endpoints (login/register/refresh) — they're anonymous
            // and protected by rate limiting instead
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (!path.StartsWith("/api/auth/"))
            {
                var cookieToken = context.Request.Cookies[CookieName];
                var headerToken = context.Request.Headers[HeaderName].FirstOrDefault();

                if (string.IsNullOrEmpty(cookieToken) || string.IsNullOrEmpty(headerToken) ||
                    !CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(cookieToken),
                        Encoding.UTF8.GetBytes(headerToken)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = " CSRF token غير صالح. الرجاء تحديث الصفحة والمحاولة مرة أخرى."
                    });
                    return;
                }
            }
        }

        await _next(context);
    }

    private static string GenerateToken()
    {
        var bytes = new byte[TokenLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes);
    }
}
