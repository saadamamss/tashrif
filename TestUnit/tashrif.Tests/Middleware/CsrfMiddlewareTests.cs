using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using tashrif.API.Middleware;

namespace tashrif.Tests.Middleware;

public class CsrfMiddlewareTests
{
    /// <summary>
    /// Generates a Base64 token that is GUARANTEED to contain '+'.
    /// Base64 alphabet: A-Z, a-z, 0-9, +, /
    /// '+' appears with ~50% probability for random bytes.
    /// We force it by choosing bytes that produce '+'.
    /// </summary>
    private static string GenerateTokenWithPlus()
    {
        // Base64 encodes 3 bytes → 4 chars. '+' appears when the
        // 6-bit value is 62. We need bytes where bits produce value 62.
        // Value 62 in binary: 111110 → byte pattern 111110xx = 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF
        // Simplest: use a known plaintext that encodes with '+'
        // ">>>" encodes to "Pj4+" in Base64
        var bytes = Encoding.UTF8.GetBytes(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Bug demonstration: WebUtility.UrlDecode converts '+' to space.
    /// Base64 tokens containing '+' will FAIL CSRF validation because:
    ///   Cookie value (raw):  "abc+def=="
    ///   UrlDecode:           "abc def=="  (+ → space)
    ///   Header value:        "abc+def=="
    ///   Comparison:          MISMATCH → 403
    /// </summary>
    [Fact]
    public void UrlDecode_Converts_Plus_To_Space_Corrupting_Base64_Token()
    {
        // Arrange: a token containing '+'
        var token = GenerateTokenWithPlus();
        token.Should().Contain("+", "token must contain + to trigger the bug");

        // Act: what the middleware does to the cookie value
        var decodedCookie = WebUtility.UrlDecode(token);

        // Assert: UrlDecode corrupts '+' into space
        decodedCookie.Should().NotBe(token,
            "UrlDecode converts '+' to space, corrupting Base64 tokens");
        decodedCookie.Should().Contain(" ",
            "the '+' character becomes a space after UrlDecode");
    }

    /// <summary>
    /// Bug demonstration: a token WITHOUT '+' works fine.
    /// This proves the bug is probabilistic (~50% of tokens).
    /// </summary>
    [Fact]
    public void UrlDecode_Does_Not_Corrupt_Token_Without_Plus()
    {
        // Arrange: a token without '+' (use '/' which also encodes but differently)
        // "AAAA" in Base64 has no '+' or '/'
        var token = Convert.ToBase64String(new byte[] { 0x00, 0x00, 0x00, 0x00 });
        token.Should().NotContain("+");

        // Act
        var decodedCookie = WebUtility.UrlDecode(token);

        // Assert: no corruption
        decodedCookie.Should().Be(token);
    }

    /// <summary>
    /// After fix: a token with '+' should be accepted.
    /// Before fix: this returned 403 because UrlDecode corrupted '+' into space.
    /// </summary>
    [Fact]
    public async Task Middleware_Accepts_Valid_Token_Containing_Plus()
    {
        // Arrange
        var token = GenerateTokenWithPlus();
        token.Should().Contain("+");

        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/jobs/publish";

        // Browser sends the raw cookie value (NOT URL-encoded)
        context.Request.Headers["Cookie"] = $"csrf_token={token}";

        // Frontend sends the same raw token in the header
        context.Request.Headers["X-CSRF-Token"] = token;

        // Track if _next was called
        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

        var middleware = new CsrfMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert: middleware should pass the request through
        context.Response.StatusCode.Should().Be(200,
            "fixed middleware compares raw values — token with '+' should be accepted");
        nextCalled.Should().BeTrue("request should pass through to next middleware");
    }

    /// <summary>
    /// Control test: token WITHOUT '+' works correctly.
    /// </summary>
    [Fact]
    public async Task Middleware_Accepts_Valid_Token_Without_Plus()
    {
        // Arrange: generate a token without '+'
        string token;
        do
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            token = Convert.ToBase64String(bytes);
        } while (token.Contains("+"));

        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/jobs/publish";

        context.Request.Headers["Cookie"] = $"csrf_token={token}";
        context.Request.Headers["X-CSRF-Token"] = token;

        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

        var middleware = new CsrfMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert: passes through
        context.Response.StatusCode.Should().Be(200);
        nextCalled.Should().BeTrue("request should pass when token has no '+'");
    }

    /// <summary>
    /// Demonstrates the概率: roughly 50% of random Base64 tokens contain '+'.
    /// </summary>
    [Fact]
    public void Base64_Tokens_Contain_Plus_About_Half_The_Time()
    {
        var withPlus = 0;
        var total = 1000;

        for (var i = 0; i < total; i++)
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            if (Convert.ToBase64String(bytes).Contains("+"))
                withPlus++;
        }

        // Should be roughly 400-600 out of 1000
        withPlus.Should().BeGreaterThan(300,
            "Base64 '+' appears in ~50% of random tokens — this proves the bug affects most requests");
        withPlus.Should().BeLessThan(700);
    }

    /// <summary>
    /// Auth endpoints are EXEMPT from CSRF — should always pass.
    /// </summary>
    [Fact]
    public async Task Middleware_Skips_Validation_For_Auth_Endpoints()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/auth/login";

        // No CSRF token at all
        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

        var middleware = new CsrfMiddleware(next);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
        nextCalled.Should().BeTrue("auth endpoints should skip CSRF validation");
    }

    /// <summary>
    /// GET requests should pass without CSRF validation.
    /// </summary>
    [Fact]
    public async Task Middleware_Skips_Validation_For_Get_Requests()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/jobs";

        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

        var middleware = new CsrfMiddleware(next);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
        nextCalled.Should().BeTrue("GET requests should skip CSRF validation");
    }

    /// <summary>
    /// Missing header should return 403.
    /// </summary>
    [Fact]
    public async Task Middleware_Rejects_Request_Missing_Header()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/jobs/publish";
        context.Request.Headers["Cookie"] = "csrf_token=sometoken";

        // No X-CSRF-Token header

        RequestDelegate next = ctx => Task.CompletedTask;
        var middleware = new CsrfMiddleware(next);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(403);
    }

    /// <summary>
    /// Mismatched header vs cookie should return 403.
    /// </summary>
    [Fact]
    public async Task Middleware_Rejects_Mismatched_Tokens()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "PUT";
        context.Request.Path = "/api/individuals/profile";
        context.Request.Headers["Cookie"] = "csrf_token=tokenA";
        context.Request.Headers["X-CSRF-Token"] = "tokenB";

        RequestDelegate next = ctx => Task.CompletedTask;
        var middleware = new CsrfMiddleware(next);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(403);
    }
}
