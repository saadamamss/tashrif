namespace tashrif.Data.Interfaces;

public interface IMessagesService
{
    Task<PaginationResultDto<MessageResponseDto>> GetByApplicationAsync(long applicationId, long userId, PaginationDto pagination);
    Task<MessageResponseDto> SendAsync(long applicationId, long userId, string body);
}
