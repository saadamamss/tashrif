using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using tashrif.Data.Constants;
using tashrif.Data.DTOs.Auth;
using tashrif.Email.Interfaces;
using tashrif.Email.Templates;
using tashrif.Identity.Interfaces;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private const string RefreshCookieName = "refresh_token";
    private const string AccessCookieName = "access_token";

    public AuthController(IAuthService authService, IEmailService emailService, IAuditService auditService)
    {
        _authService = authService;
        _emailService = emailService;
        _auditService = auditService;
    }

    /// <summary>Audit writes must never fail the user's operation — log &amp; continue.</summary>
    private async Task TryLog(long userId, string action, string entityType, long entityId,
        string? oldValue = null, string? newValue = null)
    {
        try
        {
            await _auditService.LogAsync(userId, action, entityType, entityId, oldValue, newValue,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[audit] failed to write {action} for user {userId}: {ex.Message}");
        }
    }

    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        SetAccessTokenCookie(result.Token);
        SetRefreshCookie(result.RefreshToken);
        await TryLog(result.User.Id, AuditActions.AuthLogin, "users", result.User.Id);
        return Ok(result.User);
    }

    [EnableRateLimiting("register")]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register()
    {
        if (Request.ContentType == null || !Request.ContentType.Contains("multipart/form-data"))
            throw new BadHttpRequestException("التسجيل يتطلب إرسال البيانات كـ multipart/form-data", 415);

        var hasCompanyName = Request.Form.ContainsKey("companyName");

        AuthResponseDto result;
        if (hasCompanyName)
        {
            var dto = new RegisterEntityDto
            {
                CompanyLogo = Request.Form.Files.GetFile("companyLogo"),
                CompanyName = Request.Form["companyName"]!,
                NationalId = Request.Form["nationalId"]!,
                Password = Request.Form["password"]!,
                FieldName = Request.Form["fieldName"]!,
                Sector = Request.Form["sector"]!,
                Country = Request.Form["country"]!,
                Province = Request.Form["province"]!,
                CompanyDesc = Request.Form["companyDesc"]!,
                CompanyWebsite = Request.Form["companyWebsite"]!,
                TwitterAccount = Request.Form["twitterAccount"]!,
                FacebookAccount = Request.Form["facebookAccount"]!,
                YoutubeAccount = Request.Form["youtubeAccount"]!,
                Name = Request.Form["name"]!,
                Email = Request.Form["email"]!,
                Phone = Request.Form["phone"]!,
                Role = Request.Form["role"]!,
                Nationality = Request.Form["nationality"]!,
            };
            result = await _authService.RegisterEntityAsync(dto);

            _ = Task.Run(async () =>
            {
                var email = dto.Email;
                var name = dto.CompanyName;
                await _emailService.SendAsync(email, "مرحباً بك في تشريف", EmailTemplates.Welcome(name));
            });
        }
        else
        {
            var dto = new RegisterIndividualDto
            {
                FirstName = Request.Form["firstName"]!,
                LastName = Request.Form["lastName"]!,
                NationalId = Request.Form["nationalId"]!,
                Password = Request.Form["password"]!,
                Phone = Request.Form["phone"]!,
                Email = Request.Form["email"]!,
                Gender = Request.Form["gender"]!,
                Nationality = Request.Form["nationality"]!,
                IdFile = Request.Form.Files.GetFile("idFile"),
            };
            result = await _authService.RegisterIndividualAsync(dto);

            _ = Task.Run(async () =>
            {
                var email = dto.Email;
                var name = $"{dto.FirstName} {dto.LastName}";
                await _emailService.SendAsync(email, "مرحباً بك في تشريف", EmailTemplates.Welcome(name));
            });
        }

        SetAccessTokenCookie(result.Token);
        SetRefreshCookie(result.RefreshToken);
        return Ok(result.User);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _authService.GetCurrentUserAsync(userId);
        return Ok(user);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<UserDto>> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(refreshToken))
            throw new BadHttpRequestException("Refresh token required", 401);

        var result = await _authService.RefreshTokenAsync(refreshToken);
        SetAccessTokenCookie(result.Token);
        SetRefreshCookie(result.RefreshToken);
        return Ok(result.User);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAccessTokenCookie();
        ClearRefreshCookie();
        return Ok(new { message = "تم تسجيل الخروج بنجاح" });
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<ActionResult<UserDto>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _authService.ChangePasswordAsync(userId, dto);
        // old_value/new_value stay null — never write password material to the audit trail.
        await TryLog(userId, AuditActions.AuthPasswordChanged, "users", userId);
        return Ok(user);
    }

    private void SetAccessTokenCookie(string value)
    {
        Response.Cookies.Append(AccessCookieName, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            MaxAge = TimeSpan.FromHours(2),
        });
    }

    private void ClearAccessTokenCookie()
    {
        Response.Cookies.Append(AccessCookieName, string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTimeOffset.UnixEpoch,
        });
    }

    private void SetRefreshCookie(string value)
    {
        Response.Cookies.Append(RefreshCookieName, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            MaxAge = TimeSpan.FromDays(7),
        });
    }

    private void ClearRefreshCookie()
    {
        Response.Cookies.Append(RefreshCookieName, string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTimeOffset.UnixEpoch,
        });
    }
}
