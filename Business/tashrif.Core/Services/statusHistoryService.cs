using Microsoft.EntityFrameworkCore;
using tashrif.Data.DTOs;
using tashrif.Data.Interfaces;

namespace tashrif.Core;

public class statusHistoryService(IUnitOfWork unitOfWork) : IstatusHistoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task RecordAsync(long applicationId, string? oldStatus, string newStatus, long changedBy)
    {
        var entry = new application_status_history
        {
            application_id = applicationId,
            old_status = oldStatus,
            new_status = newStatus,
            changed_by = changedBy,
            changed_at = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.Application_status_historyRepository.AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RecordBatchAsync(List<(long applicationId, string? oldStatus, string newStatus)> entries, long changedBy)
    {
        if (entries.Count == 0) return;

        var now = DateTime.UtcNow;
        foreach (var (applicationId, oldStatus, newStatus) in entries)
        {
            await _unitOfWork.Application_status_historyRepository.AddAsync(new application_status_history
            {
                application_id = applicationId,
                old_status = oldStatus,
                new_status = newStatus,
                changed_by = changedBy,
                changed_at = now,
                CreatedAt = now,
                UpdatedAt = now,
            });
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<StatusHistoryDto>> GetHistoryAsync(long applicationId, long userId)
    {
        // Verify application exists and user has access
        var appsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await appsQuery
            .Include(a => a.job_Entity)
            .FirstOrDefaultAsync(a => a.Id == applicationId && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        if (app.user_id != userId && app.job_Entity?.entity_id != userId)
            throw new UnauthorizedAccessException("غير مصرح لك بمشاهدة سجل الحالة");

        // Query history with user names (batch-join by IDs, same pattern as adminService)
        var historyQuery = await _unitOfWork.Application_status_historyRepository.GetQueryable();

        var entries = await historyQuery
            .Where(h => h.application_id == applicationId && !h.IsDeleted)
            .OrderBy(h => h.changed_at)
            .ToListAsync();

        var userIds = entries.Select(h => h.changed_by).Distinct().ToList();
        var usersMap = new Dictionary<long, string>();
        if (userIds.Count > 0)
        {
            var usersQuery = await _unitOfWork.UsersRepository.GetQueryable();
            usersMap = await usersQuery
                .Where(u => userIds.Contains(u.Id) && !u.IsDeleted)
                .Select(u => new { u.Id, u.name })
                .ToDictionaryAsync(x => x.Id, x => x.name);
        }

        return entries.Select(h => new StatusHistoryDto
        {
            Id = h.Id,
            ApplicationId = h.application_id,
            OldStatus = h.old_status,
            NewStatus = h.new_status,
            ChangedBy = h.changed_by,
            ChangedByName = usersMap.TryGetValue(h.changed_by, out var name) ? name : null,
            ChangedAt = h.changed_at,
        }).ToList();
    }
}
