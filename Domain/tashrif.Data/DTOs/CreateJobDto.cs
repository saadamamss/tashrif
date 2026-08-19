namespace tashrif.Data.DTOs;

public class CreateJobDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public int Vacancies { get; set; }
    public string Target { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Hours { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public DateTime? EndDate { get; set; }
    public List<string> Benefits { get; set; } = new();
    public List<string> Responsibilities { get; set; } = new();
    public List<string> Conditions { get; set; } = new();
}
