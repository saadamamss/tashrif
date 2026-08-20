using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IjobsService
{
    Task<PaginationResultDto<JobResponseDto>> GetAllAsync(PaginationDto pagination, string? type, string? gender, string? search, string? location = null, long? entityId = null, long? userId = null);
    Task<PaginationResultDto<JobResponseDto>> GetMyJobsAsync(PaginationDto pagination, long entityUserId, string? type = null, string? status = null);
    Task<JobResponseDto> GetJobByIdAsync(long id, long? userId = null);
    Task<JobFilterOptionsDto> GetFilterOptionsAsync();
    Task<JobResponseDto> PublishAsync(CreateJobDto dto, long entityId);
    Task<PaginationResultDto<ApplicationResponseDto>> GetApplicationsAsync(long jobId, long entityUserId, PaginationDto pagination);
}
