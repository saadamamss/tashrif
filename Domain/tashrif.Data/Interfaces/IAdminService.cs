using tashrif.Data.DTOs.Admin;
using tashrif.Data.DTOs.Auth;
using tashrif.Data.Models;

namespace tashrif.Data.Interfaces;

public interface IAdminService
{
    Task<AdminStatsDto> GetStatsAsync();
    Task<UserDto> UpdateProfileAsync(long adminId, UpdateAdminProfileDto dto);
    Task<UserDto> UpdateAvatarAsync(long adminId, string avatarUrl);
    Task<PaginationResultDto<AdminUserDto>> GetUsersAsync(AdminUserFilterDto filter);
    Task<AdminUserDto?> GetUserByIdAsync(long id);
    Task DeactivateUserAsync(long id, long adminId);
    Task ActivateUserAsync(long id);
    Task<PaginationResultDto<AdminAuditLogDto>> GetAuditLogsAsync(AdminAuditLogFilterDto filter);
    Task<PaginationResultDto<JobResponseDto>> GetAllJobsAsync(PaginationDto pagination, string? status, string? search);
    Task DeactivateJobAsync(long id);
}
