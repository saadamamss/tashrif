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
        string csrfToken;
        if (context.Request.Cookies.TryGetValue(CookieName, out var existing) && !string.IsNullOrEmpty(existing))
        {
            csrfToken = existing;
        }
        else
        {
            csrfToken = GenerateToken();
        }

        // CSRF cookie — HttpOnly, fixed for the session
        context.Response.Cookies.Append(CookieName, csrfToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
        });

        // Return token in header ONLY on auth endpoints (login, register, refresh, me)
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.StartsWith("/api/auth/"))
        {
            context.Response.Headers[HeaderName] = csrfToken;
        }

        // Validate CSRF on state-changing methods
        if (HttpMethods.IsPost(context.Request.Method) ||
            HttpMethods.IsPut(context.Request.Method) ||
            HttpMethods.IsDelete(context.Request.Method) ||
            HttpMethods.IsPatch(context.Request.Method))
        {
            if (!path.StartsWith("/api/auth/") && !path.StartsWith("/hubs/"))
            {
                var headerToken = context.Request.Headers[HeaderName].FirstOrDefault();

                // Browser sends cookies raw (RFC 6265) — no URL-encoding.
                // Compare raw values directly.
                if (string.IsNullOrEmpty(headerToken) ||
                    !CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(csrfToken),
                        Encoding.UTF8.GetBytes(headerToken)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "CSRF token غير صالح."
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
