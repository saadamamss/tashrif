using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using tashrif.Data.DTOs.Auth;
using tashrif.Identity.Interfaces;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private const string RefreshCookieName = "refresh_token";
    private const string AccessCookieName = "access_token";

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        SetAccessTokenCookie(result.Token);
        SetRefreshCookie(result.RefreshToken);
        return Ok(result.User);
    }

    [EnableRateLimiting("register")]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register()
    {
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
                CvFile = Request.Form.Files.GetFile("cvFile"),
                IdFile = Request.Form.Files.GetFile("idFile"),
            };
            result = await _authService.RegisterIndividualAsync(dto);
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
