namespace tashrif.Data.DTOs;

public class ScheduleInterviewDto
{
    public long ApplicationId { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Link { get; set; }
    public string? Notes { get; set; }
}
