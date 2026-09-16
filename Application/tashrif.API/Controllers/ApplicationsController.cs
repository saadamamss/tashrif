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
    private readonly IEmailService _emailService;
    private readonly IBackgroundTaskQueue _taskQueue;

    public ApplicationsController(IapplicationsService applicationsService, IEmailService emailService, IBackgroundTaskQueue taskQueue)
    {
        _applicationsService = applicationsService;
        _emailService = emailService;
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

                if (result.Job?.EntityEmail != null)
                {
                    var entityEmail = result.Job.EntityEmail;
                    var entityName = result.Job.EntityName;
                    var applicantName = result.UserName ?? "متقدم";
                    await _emailService.SendAsync(
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

    [Authorize(Policy = "Individual")]
    [HttpPut("{id}/withdraw")]
    public async Task<ActionResult<ApplicationResponseDto>> Withdraw(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _applicationsService.WithdrawAsync(id, userId);
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
}
