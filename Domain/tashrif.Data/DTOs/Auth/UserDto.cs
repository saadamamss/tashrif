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
    public bool MustChangePassword { get; set; }
}
