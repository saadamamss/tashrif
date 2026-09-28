using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tashrif.Data.Constants;
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
    private readonly IAuditService _auditService;

    public AdminController(IAdminService adminService, IFileStorageService fileStorage, IAuditService auditService)
    {
        _adminService = adminService;
        _fileStorage = fileStorage;
        _auditService = auditService;
    }

    /// <summary>Audit writes must never fail the user's operation — log &amp; continue.</summary>
    private async Task TryLog(long userId, string action, string entityType, long entityId,
        string? oldValue = null, string? newValue = null)
    {
        try
        {
            await _auditService.LogAsync(userId, action, entityType, entityId, oldValue, newValue,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[audit] failed to write {action} for user {userId}: {ex.Message}");
        }
    }

    [HttpPut("profile")]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateAdminProfileDto dto)
    {
        var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.UpdateProfileAsync(adminId, dto);
        await TryLog(adminId, AuditActions.AdminProfileUpdated, "users", adminId);
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
        var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _adminService.DeactivateUserAsync(id, adminId);
        await TryLog(adminId, AuditActions.AdminUserDeactivated, "users", id, "active", "deactivated");
        return Ok(new { message = "تم تعطيل الحساب بنجاح" });
    }

    [HttpPut("users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(long id)
    {
        var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _adminService.ActivateUserAsync(id);
        await TryLog(adminId, AuditActions.AdminUserActivated, "users", id, "deactivated", "active");
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
        var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        // Read the REAL previous status before the update — never guess "active".
        var previousStatus = await _adminService.GetJobStatusAsync(id);
        await _adminService.DeactivateJobAsync(id);
        await TryLog(adminId, AuditActions.AdminJobDeactivated, "jobs", id, previousStatus, "closed");
        return Ok(new { message = "تم تعطيل الوظيفة بنجاح" });
    }
}
