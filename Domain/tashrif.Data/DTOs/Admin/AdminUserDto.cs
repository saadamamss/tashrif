namespace tashrif.Data.DTOs.Admin;

public class AdminUserDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Phone { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
