using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using tashrif.API.Services;
using tashrif.Data.Constants;
using tashrif.Data.DTOs.Auth;
using tashrif.Data.Validation;
using tashrif.Email.Interfaces;
using tashrif.Email.Templates;
using tashrif.Identity.Interfaces;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAuditService _auditService;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IConfiguration _config;
    private const string RefreshCookieName = "refresh_token";
    private const string AccessCookieName = "access_token";

    /// <summary>
    /// The message returned by <c>POST /api/auth/resend-verification</c> — ALWAYS the
    /// same, whatever the real outcome (D8). A different message for "no such account"
    /// would be an enumeration oracle.
    /// </summary>
    private const string GenericResendMessage = "إذا كان الحساب موجوداً، فسيتم إرسال رسالة تفعيل جديدة.";

    /// <summary>
    /// NOTE: no <c>IEmailService</c> is injected here on purpose. The verification mail
    /// is sent from a background task, which must resolve the scoped email service from
    /// ITS OWN scope — a request-scoped instance captured by the closure would already
    /// be disposed by the time the task runs.
    /// </summary>
    public AuthController(
        IAuthService authService,
        IAuditService auditService,
        IBackgroundTaskQueue taskQueue,
        IConfiguration config)
    {
        _authService = authService;
        _auditService = auditService;
        _taskQueue = taskQueue;
        _config = config;
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
    public async Task<ActionResult<object>> Register()
    {
        if (Request.ContentType == null || !Request.ContentType.Contains("multipart/form-data"))
            throw new BadHttpRequestException("التسجيل يتطلب إرسال البيانات كـ multipart/form-data", 415);

        var hasCompanyName = Request.Form.ContainsKey("companyName");

        RegisterResponseDto result;
        if (hasCompanyName)
        {
            var dto = new RegisterEntityDto
            {
                CompanyLogo = Request.Form.Files.GetFile("companyLogo"),
                CompanyName = Request.Form["companyName"]!,
                NationalId = Request.Form["nationalId"]!,
                CommercialReg = Request.Form["commercialReg"]!,
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
            // Phase 3.7: Register() builds the DTO by hand from Request.Form, so [ApiController]
            // auto-validation never fires — enforce the DTO's DataAnnotations explicitly (both branches).
            RequestValidator.Validate(dto);
            result = await _authService.RegisterEntityAsync(dto);
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
            // Phase 3.7: enforce the DTO's DataAnnotations — hand-built DTOs skip [ApiController] validation.
            RequestValidator.Validate(dto);
            result = await _authService.RegisterIndividualAsync(dto);
        }

        // D3: no session is created — the account must be verified first. The raw token
        // (D13) goes straight into the mail link and is NEVER returned to the browser.
        EnqueueVerificationMail(result);

        return StatusCode(StatusCodes.Status201Created, new { message = result.Message });
    }

    /// <summary>
    /// A1 — the link targets the FRONTEND page, not the API. An email scanner that
    /// prefetches the URL must not burn the single-use token.
    /// </summary>
    private string BuildVerificationLink(string rawToken)
    {
        var baseUrl = (_config["Frontend:BaseUrl"] ?? "http://localhost:3000").TrimEnd('/');
        return $"{baseUrl}/verify-email?token={Uri.EscapeDataString(rawToken)}";
    }

    /// <summary>
    /// D13/D14 — enqueues the verification mail and records the delivery outcome on the
    /// token row (<c>sent_at</c> / <c>last_error</c>).
    ///
    /// <para>
    /// The scoped <c>IEmailService</c> and <c>IUnitOfWork</c> are resolved from the task
    /// scope, NOT captured from the request scope — the request scope is already disposed
    /// by the time the background task runs. (This also fixes the pre-existing
    /// scoped-service capture in this controller; the identical bug in
    /// <c>ContractsController</c>/<c>InterviewsController</c> is tracked separately.)
    /// </para>
    /// </summary>
    private void EnqueueVerificationMail(RegisterResponseDto result)
    {
        var link = BuildVerificationLink(result.VerificationToken);
        var to = result.Email;
        var name = result.Name;
        var tokenId = result.VerificationTokenId;

        _taskQueue.Enqueue(async scope =>
        {
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var uow = scope.ServiceProvider.GetRequiredService<tashrif.Data.Interfaces.IUnitOfWork>();
            var clock = scope.ServiceProvider.GetRequiredService<tashrif.Data.Interfaces.IClock>();

            var (sent, error) = await emailService.TrySendAsync(
                to,
                "تأكيد بريدك الإلكتروني - تشريف",
                EmailTemplates.VerifyEmail(name, link));

            // D14: record what actually happened. A swallowed SMTP failure must not
            // leave the user locked out with no signal.
            var token = await uow.Email_verification_tokensRepository.GetByIdAsync(tokenId);
            if (token != null)
            {
                token.sent_at = sent ? clock.UtcNow : null;
                token.last_error = sent ? null : error;
                token.UpdatedAt = clock.UtcNow;
                uow.Email_verification_tokensRepository.Update(token);
                await uow.SaveChangesAsync();
            }
        });
    }

    /// <summary>
    /// A1/D7 — redeems a verification token and signs the user in (auto-login).
    ///
    /// <para>
    /// Called by the frontend verification page with the token from the query string —
    /// the email link points at the PAGE, not at this endpoint, so an email scanner
    /// that prefetches the link cannot burn the single-use token.
    /// </para>
    /// </summary>
    [EnableRateLimiting("verify")]
    [HttpGet("verify-email")]
    public async Task<ActionResult<UserDto>> VerifyEmail([FromQuery] string token)
    {
        var result = await _authService.VerifyEmailAsync(token);
        SetAccessTokenCookie(result.Token);
        SetRefreshCookie(result.RefreshToken);
        await TryLog(result.User.Id, AuditActions.AuthLogin, "users", result.User.Id, null, "email_verified");
        return Ok(result.User);
    }

    /// <summary>
    /// D8 — re-sends the verification mail.
    ///
    /// <para>
    /// The response is ALWAYS the same generic message, whatever actually happened
    /// (unknown id, deactivated account, already-verified account, or success) — a
    /// different answer would let an attacker enumerate which national ids exist.
    /// </para>
    /// </summary>
    [EnableRateLimiting("resend")]
    [HttpPost("resend-verification")]
    public async Task<ActionResult<object>> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        var result = await _authService.ResendVerificationAsync(dto.NationalId);

        if (result.ShouldSend)
        {
            EnqueueVerificationMail(new RegisterResponseDto
            {
                Message = GenericResendMessage,
                VerificationToken = result.VerificationToken,
                VerificationTokenId = result.VerificationTokenId,
                Email = result.Email,
                Name = result.Name,
            });
        }

        return Ok(new { message = GenericResendMessage });
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
