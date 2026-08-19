using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IapplicationsService
{
    Task<PaginationResultDto<ApplicationResponseDto>> GetAllAsync(PaginationDto pagination, long userId, string? userType, string? status, string? search = null);
    Task<ApplicationResponseDto> GetByIdAsync(long id, long userId);
    Task<ApplicationResponseDto> ApplyAsync(ApplyJobDto dto, long userId);
    Task<ApplicationResponseDto> UpdateStatusAsync(long id, string status, long userId);
    Task DeleteAsync(long id, long userId);
}
