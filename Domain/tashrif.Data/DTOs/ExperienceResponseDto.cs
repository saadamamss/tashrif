namespace tashrif.Data.DTOs;

public class ExperienceResponseDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Employer { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
