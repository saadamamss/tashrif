namespace tashrif.Data.Interfaces;

public interface IstatusHistoryService
{
    Task RecordAsync(long applicationId, string? oldStatus, string newStatus, long changedBy);
    Task RecordBatchAsync(List<(long applicationId, string? oldStatus, string newStatus)> entries, long changedBy);
    Task<List<StatusHistoryDto>> GetHistoryAsync(long applicationId, long userId);
}
