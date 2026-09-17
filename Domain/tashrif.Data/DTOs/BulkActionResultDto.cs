namespace tashrif.Data.DTOs;

public class BulkActionResultDto
{
    public long ApplicationId { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
}
