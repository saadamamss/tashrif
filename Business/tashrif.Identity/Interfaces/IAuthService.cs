using tashrif.Data.DTOs.Auth;

namespace tashrif.Identity.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterIndividualAsync(RegisterIndividualDto dto);
    Task<AuthResponseDto> RegisterEntityAsync(RegisterEntityDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<UserDto> GetCurrentUserAsync(long userId);
    Task<UserDto> ChangePasswordAsync(long userId, ChangePasswordDto dto);
}
