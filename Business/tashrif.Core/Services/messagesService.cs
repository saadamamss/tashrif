using Microsoft.EntityFrameworkCore;

namespace tashrif.Core.Services;

public class MessagesService : IMessagesService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResultDto<MessageResponseDto>> GetByApplicationAsync(long applicationId, long userId, PaginationDto pagination)
    {
        // Verify the user has access to this application
        var appsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await appsQuery
            .Include(a => a.job_Entity)
            .FirstOrDefaultAsync(a => a.Id == applicationId && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        var isOwner = app.user_id == userId;
        var isEntity = app.job_Entity != null && app.job_Entity.entity_id == userId;
        if (!isOwner && !isEntity)
            throw new UnauthorizedAccessException("لا تملك صلاحية الوصول لهذه المحادثة");

        var query = await _unitOfWork.MessagesRepository.GetQueryable();
        query = query.Where(m => m.application_id == applicationId && !m.IsDeleted);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(m => m.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .Select(m => new MessageResponseDto
            {
                Id = m.Id,
                ApplicationId = m.application_id,
                SenderId = m.sender_id,
                SenderName = m.sender_entity!.name,
                Body = m.body,
                IsRead = m.is_read,
                CreatedAt = m.CreatedAt,
            })
            .ToListAsync();

        return new PaginationResultDto<MessageResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    public async Task<MessageResponseDto> SendAsync(long applicationId, long userId, string body)
    {
        // Verify the user has access to this application
        var appsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await appsQuery
            .Include(a => a.job_Entity)
            .FirstOrDefaultAsync(a => a.Id == applicationId && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        var isOwner = app.user_id == userId;
        var isEntity = app.job_Entity != null && app.job_Entity.entity_id == userId;
        if (!isOwner && !isEntity)
            throw new UnauthorizedAccessException("لا تملك صلاحية إرسال رسائل في هذه المحادثة");

        var message = new messages
        {
            application_id = applicationId,
            sender_id = userId,
            body = body,
            is_read = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.MessagesRepository.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        // Fetch sender name
        var usersQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var sender = await usersQuery.FirstOrDefaultAsync(u => u.Id == userId);

        return new MessageResponseDto
        {
            Id = message.Id,
            ApplicationId = message.application_id,
            SenderId = message.sender_id,
            SenderName = sender?.name ?? "",
            Body = message.body,
            IsRead = message.is_read,
            CreatedAt = message.CreatedAt,
        };
    }
}
