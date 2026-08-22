namespace tashrif.Data.DTOs;

public class IndividualProfileResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? District { get; set; }
    public string? Street { get; set; }
    public string? Zipcode { get; set; }
    public string? JobTitle { get; set; }
    public string? AvatarUrl { get; set; }
    public short ProfileCompletionPct { get; set; }
    public string? CvFile { get; set; }
    public string? IdFile { get; set; }
    public IndividualStatsDto? Stats { get; set; }
}
