using System.Text.Json.Serialization;

namespace tashrif.Data.DTOs;

public class JobResponseDto
{
    public long Id { get; set; }
    public long EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string? EntityLogo { get; set; }
    public string? EntityEmail { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public int Vacancies { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public List<string> Benefits { get; set; } = new();
    public List<string> Responsibilities { get; set; } = new();
    public List<string> Conditions { get; set; } = new();
    public string Gender { get; set; } = string.Empty;
    public string Hours { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? PublishDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsApplied { get; set; }
    public int ApplicantCount { get; set; }
}
