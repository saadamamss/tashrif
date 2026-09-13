using tashrif.Data.Interfaces;
using tashrif.Data;

namespace tashrif.Core.Services;

public class AuditService : IAuditService
{
    private readonly IGenericRepository<audit_logs> _auditRepo;
    private readonly IUnitOfWork _uow;

    public AuditService(IGenericRepository<audit_logs> auditRepo, IUnitOfWork uow)
    {
        _auditRepo = auditRepo;
        _uow = uow;
    }

    public async Task LogAsync(long userId, string action, string entityType, long entityId,
        string? oldValue = null, string? newValue = null,
        string? ipAddress = null, string? userAgent = null)
    {
        var log = new audit_logs
        {
            user_id = userId,
            action = action,
            entity_type = entityType,
            entity_id = entityId,
            old_value = oldValue,
            new_value = newValue,
            ip_address = ipAddress,
            user_agent = userAgent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _auditRepo.AddAsync(log);
        await _uow.SaveChangesAsync();
    }
}
