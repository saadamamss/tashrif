using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IinterviewsService
{
    Task<PaginationResultDto<InterviewResponseDto>> GetAllAsync(PaginationDto pagination, long userId, string? userType);
    Task<InterviewResponseDto> ScheduleAsync(ScheduleInterviewDto dto, long entityId);
}
