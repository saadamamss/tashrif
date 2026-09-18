using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tashrif.Data.DTOs.Admin;
using tashrif.Data.DTOs.Auth;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IFileStorageService _fileStorage;

    public AdminController(IAdminService adminService, IFileStorageService fileStorage)
    {
        _adminService = adminService;
        _fileStorage = fileStorage;
    }

    [HttpPut("profile")]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateAdminProfileDto dto)
    {
        var adminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.UpdateProfileAsync(adminId, dto);
        return Ok(result);
    }

    [HttpPut("profile/avatar")]
    public async Task<ActionResult<UserDto>> UpdateAvatar(IFormFile file)
    {
        var adminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var avatarUrl = await _fileStorage.SaveFileAsync(file, "avatars");
        var result = await _adminService.UpdateAvatarAsync(adminId, avatarUrl);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<AdminStatsDto>> GetStats()
    {
        var result = await _adminService.GetStatsAsync();
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<ActionResult<PaginationResultDto<AdminUserDto>>> GetUsers([FromQuery] AdminUserFilterDto filter)
    {
        var result = await _adminService.GetUsersAsync(filter);
        return Ok(result);
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<AdminUserDto>> GetUserById(long id)
    {
        var result = await _adminService.GetUserByIdAsync(id);
        if (result == null)
            throw new KeyNotFoundException("المستخدم غير موجود");
        return Ok(result);
    }

    [HttpPut("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(long id)
    {
        var adminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _adminService.DeactivateUserAsync(id, adminId);
        return Ok(new { message = "تم تعطيل الحساب بنجاح" });
    }

    [HttpPut("users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(long id)
    {
        await _adminService.ActivateUserAsync(id);
        return Ok(new { message = "تم تفعيل الحساب بنجاح" });
    }

    [HttpGet("audit-logs")]
    public async Task<ActionResult<PaginationResultDto<AdminAuditLogDto>>> GetAuditLogs([FromQuery] AdminAuditLogFilterDto filter)
    {
        var result = await _adminService.GetAuditLogsAsync(filter);
        return Ok(result);
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<PaginationResultDto<JobResponseDto>>> GetAllJobs(
        [FromQuery] PaginationDto pagination,
        [FromQuery] string? status,
        [FromQuery] string? search)
    {
        var result = await _adminService.GetAllJobsAsync(pagination, status, search);
        return Ok(result);
    }

    [HttpPut("jobs/{id}/deactivate")]
    public async Task<IActionResult> DeactivateJob(long id)
    {
        await _adminService.DeactivateJobAsync(id);
        return Ok(new { message = "تم تعطيل الوظيفة بنجاح" });
    }
}
