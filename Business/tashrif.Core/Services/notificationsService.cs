using Microsoft.EntityFrameworkCore;

namespace tashrif.Core.Services;

public class NotificationsService : INotificationsService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationResponseDto> CreateAsync(long userId, string title, string? body, string type, long? referenceId = null, string? referenceType = null)
    {
        var notification = new notifications
        {
            user_id = userId,
            title = title,
            body = body,
            type = type,
            reference_id = referenceId,
            reference_type = referenceType,
            is_read = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.NotificationsRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(notification);
    }

    public async Task<PaginationResultDto<NotificationResponseDto>> GetByUserAsync(long userId, PaginationDto pagination, bool? unreadOnly = null)
    {
        var query = await _unitOfWork.NotificationsRepository.GetQueryable();
        query = query.Where(n => n.user_id == userId && !n.IsDeleted);

        if (unreadOnly == true)
            query = query.Where(n => !n.is_read);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                UserId = n.user_id,
                Title = n.title,
                Body = n.body,
                Type = n.type,
                ReferenceId = n.reference_id,
                ReferenceType = n.reference_type,
                IsRead = n.is_read,
                CreatedAt = n.CreatedAt,
            })
            .ToListAsync();

        return new PaginationResultDto<NotificationResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    public async Task<int> GetUnreadCountAsync(long userId)
    {
        var query = await _unitOfWork.NotificationsRepository.GetQueryable();
        return await query.CountAsync(n => n.user_id == userId && !n.is_read && !n.IsDeleted);
    }

    public async Task<NotificationResponseDto> MarkReadAsync(long id, long userId)
    {
        var query = await _unitOfWork.NotificationsRepository.GetQueryable();
        var notification = await query
            .FirstOrDefaultAsync(n => n.Id == id && n.user_id == userId && !n.IsDeleted)
            ?? throw new KeyNotFoundException("الإشعار غير موجود");

        notification.is_read = true;
        notification.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.NotificationsRepository.Update(notification);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(notification);
    }

    public async Task MarkAllReadAsync(long userId)
    {
        var query = await _unitOfWork.NotificationsRepository.GetQueryable();
        var unread = await query
            .Where(n => n.user_id == userId && !n.is_read && !n.IsDeleted)
            .ToListAsync();

        foreach (var n in unread)
        {
            n.is_read = true;
            n.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.NotificationsRepository.Update(n);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private static NotificationResponseDto MapToDto(notifications n)
    {
        return new NotificationResponseDto
        {
            Id = n.Id,
            UserId = n.user_id,
            Title = n.title,
            Body = n.body,
            Type = n.type,
            ReferenceId = n.reference_id,
            ReferenceType = n.reference_type,
            IsRead = n.is_read,
            CreatedAt = n.CreatedAt,
        };
    }
}
