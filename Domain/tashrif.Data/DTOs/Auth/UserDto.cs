namespace tashrif.Data.DTOs.Auth;

public class UserDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? NationalId { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// False until the email on this account is proven (D2). The frontend uses it to
    /// route an authenticated-but-unverified user to the verification page instead of
    /// logging them out (B6/F4).
    /// </summary>
    public bool IsEmailVerified { get; set; }
}
