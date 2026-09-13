namespace tashrif.Data.Interfaces;

public interface IAuditService
{
    Task LogAsync(long userId, string action, string entityType, long entityId,
        string? oldValue = null, string? newValue = null,
        string? ipAddress = null, string? userAgent = null);
}
