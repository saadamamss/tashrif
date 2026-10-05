using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IinterviewsService
{
    Task<PaginationResultDto<InterviewResponseDto>> GetAllAsync(PaginationDto pagination, long userId, string? userType, long? applicationId = null);
    Task<InterviewResponseDto> ScheduleAsync(ScheduleInterviewDto dto, long entityId);
}
