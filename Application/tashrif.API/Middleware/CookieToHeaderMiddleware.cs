namespace tashrif.API.Middleware;

public class CookieToHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public CookieToHeaderMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
                context.Request.Headers["Authorization"] = "Bearer " + token;
        }
        await _next(context);
    }
}
