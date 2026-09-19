using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tashrif.API.Services;
using tashrif.Email.Interfaces;
using tashrif.Email.Templates;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly IapplicationsService _applicationsService;
    private readonly IstatusHistoryService _statusHistoryService;
    private readonly IBackgroundTaskQueue _taskQueue;

    public ApplicationsController(IapplicationsService applicationsService, IstatusHistoryService statusHistoryService, IBackgroundTaskQueue taskQueue)
    {
        _applicationsService = applicationsService;
        _statusHistoryService = statusHistoryService;
        _taskQueue = taskQueue;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<ApplicationResponseDto>>> GetAll(
        [FromQuery] PaginationDto pagination,
        [FromQuery] string? status,
        [FromQuery] string? search)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userType = User.FindFirst(ClaimTypes.Role)?.Value;
        var result = await _applicationsService.GetAllAsync(pagination, userId, userType, status, search);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationResponseDto>> GetById(long id)
    {
        try
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _applicationsService.GetByIdAsync(id, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "الطلب غير موجود" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [Authorize(Policy = "Individual")]
    [HttpPost("apply")]
    public async Task<ActionResult<ApplicationResponseDto>> Apply([FromBody] ApplyJobDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _applicationsService.ApplyAsync(dto, userId);

            _taskQueue.Enqueue(async scope =>
            {
                var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
                var notificationHub = scope.ServiceProvider.GetRequiredService<INotificationHubService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                if (result.Job?.EntityEmail != null)
                {
                    var entityEmail = result.Job.EntityEmail;
                    var entityName = result.Job.EntityName;
                    var applicantName = result.UserName ?? "متقدم";
                    await emailService.SendAsync(
                        entityEmail,
                        "متقدم جديد",
                        EmailTemplates.NewApplicant(entityName, applicantName, result.Job.Title));
                }

                var notification = await notificationsService.CreateAsync(
                    result.Job!.EntityId,
                    "متقدم جديد",
                    $"{result.UserName} قام بالتقديم على {result.Job!.Title}",
                    "new_application",
                    result.Id,
                    "application");

                await notificationHub.SendToUserAsync(result.Job!.EntityId, "NewApplication", new
                {
                    notification.Id,
                    ApplicationId = result.Id,
                    JobTitle = result.Job.Title,
                    ApplicantName = result.UserName,
                });
            });

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "Entity")]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApplicationResponseDto>> UpdateStatus(long id, [FromBody] UpdateApplicationStatusDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _applicationsService.UpdateStatusAsync(id, dto.Status, userId);

            if (dto.Status is "shortlisted" or "refused")
            {
                _taskQueue.Enqueue(async scope =>
                {
                    var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
                    var notificationHub = scope.ServiceProvider.GetRequiredService<INotificationHubService>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var isShortlist = dto.Status == "shortlisted";
                    var title = isShortlist ? "تم ترشحك" : "تم رفض طلبك";
                    var body = isShortlist
                        ? $"تم ترشحك لوظيفة {result.Job?.Title} في {result.Job?.EntityName}"
                        : $"تم رفض طلبك لوظيفة {result.Job?.Title} في {result.Job?.EntityName}";
                    var notifType = isShortlist ? "shortlisted" : "refused";

                    var notification = await notificationsService.CreateAsync(
                        result.UserId, title, body, notifType, result.Id, "application");

                    await notificationHub.SendToUserAsync(result.UserId, "StatusChanged", new
                    {
                        notification.Id,
                        ApplicationId = result.Id,
                        Status = dto.Status,
                        JobTitle = result.Job?.Title,
                    });

                    var usersQuery = await unitOfWork.UsersRepository.GetQueryable();
                    var user = await usersQuery.FirstOrDefaultAsync(u => u.Id == result.UserId);
                    if (user?.email != null && result.Job != null)
                    {
                        var emailTemplate = isShortlist
                            ? EmailTemplates.Shortlisted(result.UserName, result.Job.Title, result.Job.EntityName)
                            : EmailTemplates.ApplicationRefused(result.UserName, result.Job.Title, result.Job.EntityName);
                        var emailSubject = isShortlist ? "تم ترشحك" : "تم رفض طلبك";
                        await emailService.SendAsync(user.email, emailSubject, emailTemplate);
                    }
                });
            }

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "الطلب غير موجود" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "Entity")]
    [HttpPost("bulk-action")]
    public async Task<ActionResult<List<BulkActionResultDto>>> BulkAction([FromBody] BulkActionDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _applicationsService.BulkActionAsync(dto.ApplicationIds, dto.Action, userId);

            if (dto.Action is "shortlist" or "refuse" or "restore")
            {
                var succeededIds = result.Where(r => r.Success).Select(r => r.ApplicationId).ToList();
                if (succeededIds.Count > 0)
                {
                    _taskQueue.Enqueue(async scope =>
                    {
                        var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
                        var notificationHub = scope.ServiceProvider.GetRequiredService<INotificationHubService>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        var usersQuery = await unitOfWork.UsersRepository.GetQueryable();

                        var appsQuery = await unitOfWork.ApplicationsRepository.GetQueryable();
                        var apps = await appsQuery
                            .Include(a => a.user_Entity)
                            .Include(a => a.job_Entity)
                            .ThenInclude(j => j.entity_Entity)
                            .Where(a => succeededIds.Contains(a.Id))
                            .ToListAsync();

                        foreach (var app in apps)
                        {
                            var (title, body, notifType, emailSubject, emailTemplate) = dto.Action switch
                            {
                                "shortlist" => (
                                    "تم ترشحك",
                                    $"تم ترشحك لوظيفة {app.job_Entity?.title} في {app.job_Entity?.entity_Entity?.name}",
                                    "shortlisted",
                                    "تم ترشحك",
                                    EmailTemplates.Shortlisted(app.user_Entity?.name ?? "", app.job_Entity?.title ?? "", app.job_Entity?.entity_Entity?.name ?? "")),
                                "restore" => (
                                    "تمت إعادة تقييمك",
                                    $"تمت إعادة تقييمك لوظيفة {app.job_Entity?.title} في {app.job_Entity?.entity_Entity?.name}",
                                    "restored",
                                    "تمت إعادة تقييمك",
                                    EmailTemplates.ApplicationRestored(app.user_Entity?.name ?? "", app.job_Entity?.title ?? "", app.job_Entity?.entity_Entity?.name ?? "")),
                                _ => (
                                    "تم رفض طلبك",
                                    $"تم رفض طلبك لوظيفة {app.job_Entity?.title} في {app.job_Entity?.entity_Entity?.name}",
                                    "refused",
                                    "تم رفض طلبك",
                                    EmailTemplates.ApplicationRefused(app.user_Entity?.name ?? "", app.job_Entity?.title ?? "", app.job_Entity?.entity_Entity?.name ?? "")),
                            };

                            var notification = await notificationsService.CreateAsync(
                                app.user_id, title, body, notifType, app.Id, "application");

                            await notificationHub.SendToUserAsync(app.user_id, "StatusChanged", new
                            {
                                notification.Id,
                                ApplicationId = app.Id,
                                Status = dto.Action,
                                JobTitle = app.job_Entity?.title,
                            });

                            var user = await usersQuery.FirstOrDefaultAsync(u => u.Id == app.user_id);
                            if (user?.email != null)
                            {
                                await emailService.SendAsync(user.email, emailSubject, emailTemplate);
                            }
                        }
                    });
                }
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "Individual")]
    [HttpPut("{id}/withdraw")]
    public async Task<ActionResult<ApplicationResponseDto>> Withdraw(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _applicationsService.WithdrawAsync(id, userId);

            _taskQueue.Enqueue(async scope =>
            {
                var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
                var notificationHub = scope.ServiceProvider.GetRequiredService<INotificationHubService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                if (result.Job != null)
                {
                    var notification = await notificationsService.CreateAsync(
                        result.Job.EntityId,
                        "تم سحب الطلب",
                        $"{result.UserName} قام بسحب طلبه لوظيفة {result.Job.Title}",
                        "withdrawn",
                        result.Id,
                        "application");

                    await notificationHub.SendToUserAsync(result.Job.EntityId, "ApplicationWithdrawn", new
                    {
                        notification.Id,
                        ApplicationId = result.Id,
                        JobTitle = result.Job.Title,
                        ApplicantName = result.UserName,
                    });

                    if (result.Job.EntityEmail != null)
                    {
                        await emailService.SendAsync(
                            result.Job.EntityEmail,
                            "تم سحب الطلب",
                            EmailTemplates.ApplicationWithdrawn(result.Job.EntityName, result.UserName, result.Job.Title));
                    }
                }
            });

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "الطلب غير موجود" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _applicationsService.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "الطلب غير موجود" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id}/status-history")]
    public async Task<ActionResult<List<StatusHistoryDto>>> GetStatusHistory(long id)
    {
        try
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _statusHistoryService.GetHistoryAsync(id, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "الطلب غير موجود" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }
}
