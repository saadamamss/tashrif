namespace tashrif.Data.DTOs;

public class StatusHistoryDto
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public long ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public DateTime ChangedAt { get; set; }
}
