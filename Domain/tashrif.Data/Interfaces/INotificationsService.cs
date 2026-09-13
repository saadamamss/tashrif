namespace tashrif.Data.Interfaces;

public interface INotificationsService
{
    Task<NotificationResponseDto> CreateAsync(long userId, string title, string? body, string type, long? referenceId = null, string? referenceType = null);
    Task<PaginationResultDto<NotificationResponseDto>> GetByUserAsync(long userId, PaginationDto pagination, bool? unreadOnly = null);
    Task<int> GetUnreadCountAsync(long userId);
    Task<NotificationResponseDto> MarkReadAsync(long id, long userId);
    Task MarkAllReadAsync(long userId);
}
