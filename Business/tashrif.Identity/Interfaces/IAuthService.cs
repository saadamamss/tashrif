using tashrif.Data.DTOs.Auth;

namespace tashrif.Identity.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registers an individual and issues a verification token (D13). No session is
    /// created — the account is unverified until the token is redeemed.
    /// </summary>
    Task<RegisterResponseDto> RegisterIndividualAsync(RegisterIndividualDto dto);

    /// <summary>Registers an entity and issues a verification token (D13).</summary>
    Task<RegisterResponseDto> RegisterEntityAsync(RegisterEntityDto dto);

    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<UserDto> GetCurrentUserAsync(long userId);
    Task<UserDto> ChangePasswordAsync(long userId, ChangePasswordDto dto);

    /// <summary>
    /// D7/D11 — redeems a raw verification token, marks the email verified and returns
    /// a session (auto-login). Throws 400 for an unknown/used/expired token.
    /// </summary>
    Task<AuthResponseDto> VerifyEmailAsync(string rawToken);

    /// <summary>
    /// D8/D11 — rotates the verification token for the account with the given national
    /// id. The real outcome is returned so the controller can mail the user; the public
    /// endpoint always answers with the same generic message.
    /// </summary>
    Task<ResendVerificationResultDto> ResendVerificationAsync(string nationalId);
}
