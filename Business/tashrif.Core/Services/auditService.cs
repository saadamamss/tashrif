using tashrif.Data.Interfaces;
using tashrif.Data;
using tashrif.Data.Constants;

namespace tashrif.Core.Services;

public class AuditService : IAuditService
{
    /// <summary>old_value/new_value are capped so a huge payload can never bloat the row.</summary>
    private const int MaxValueLength = 1000;

    private readonly IGenericRepository<audit_logs> _auditRepo;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public AuditService(IGenericRepository<audit_logs> auditRepo, IUnitOfWork uow, IClock clock)
    {
        _auditRepo = auditRepo;
        _uow = uow;
        _clock = clock;
    }

    // Invariant: LogAsync saves immediately on the shared request-scoped UnitOfWork —
    // call it only AFTER the business operation has completed and saved, never mid-operation,
    // or it would commit half-finished work. Callers wrap it in try/catch so an audit
    // failure never fails the user's operation.
    public async Task LogAsync(long userId, string action, string entityType, long entityId,
        string? oldValue = null, string? newValue = null,
        string? ipAddress = null, string? userAgent = null)
    {
        if (!AuditActions.EntityTypes.Contains(entityType))
        {
            var allowed = string.Join(", ", AuditActions.EntityTypes);
            throw new InvalidOperationException(
                $"Invalid audit entity_type '{entityType}'. Allowed: {allowed}");
        }

        var log = new audit_logs
        {
            user_id = userId,
            action = action,
            entity_type = entityType,
            entity_id = entityId,
            old_value = Truncate(oldValue),
            new_value = Truncate(newValue),
            ip_address = Truncate(ipAddress),
            user_agent = Truncate(userAgent),
            CreatedAt = _clock.UtcNow,
            UpdatedAt = _clock.UtcNow,
        };

        await _auditRepo.AddAsync(log);
        await _uow.SaveChangesAsync();
    }

    private static string? Truncate(string? value) =>
        string.IsNullOrEmpty(value) || value.Length <= MaxValueLength ? value : value[..MaxValueLength];
}
