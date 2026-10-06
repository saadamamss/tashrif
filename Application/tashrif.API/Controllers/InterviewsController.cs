using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tashrif.API.Services;
using tashrif.Data.Constants;
using tashrif.Email.Interfaces;
using tashrif.Email.Templates;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize]
[Route("api/interviews")]
public class InterviewsController : ControllerBase
{
    private readonly IinterviewsService _interviewsService;
    private readonly IEmailService _emailService;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IAuditService _auditService;

    public InterviewsController(IinterviewsService interviewsService, IEmailService emailService, IBackgroundTaskQueue taskQueue, IAuditService auditService)
    {
        _interviewsService = interviewsService;
        _emailService = emailService;
        _taskQueue = taskQueue;
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

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<InterviewResponseDto>>> GetAll([FromQuery] PaginationDto pagination, [FromQuery] long? applicationId, [FromQuery] string? status)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userType = User.FindFirst(ClaimTypes.Role)?.Value;
        var result = await _interviewsService.GetAllAsync(pagination, userId, userType, applicationId, status);
        return Ok(result);
    }

    [Authorize(Policy = "Entity")]
    [HttpPost("schedule")]
    public async Task<ActionResult<InterviewResponseDto>> Schedule([FromBody] ScheduleInterviewDto dto)
    {
        var entityId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _interviewsService.ScheduleAsync(dto, entityId);
        await TryLog(entityId, AuditActions.InterviewScheduled, "interviews", result.Id, null, "scheduled");

        _taskQueue.Enqueue(async scope =>
        {
            var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
            var notificationHub = scope.ServiceProvider.GetRequiredService<INotificationHubService>();

            if (result.UserEmail != null)
            {
                var applicantEmail = result.UserEmail;
                var applicantName = result.UserName;
                await _emailService.SendAsync(applicantEmail, "موعد مقابلة العمل", EmailTemplates.InterviewScheduled(applicantName, result.JobTitle!, result.Date.ToString(), result.Time, result.Location!));
            }

            var notification = await notificationsService.CreateAsync(
                result.UserId,
                "مقابلة مجدولة",
                $"مقابلة لوظيفة {result.JobTitle} بتاريخ {result.Date}",
                "interview_scheduled",
                result.Id,
                "interview");

            await notificationHub.SendToUserAsync(result.UserId, "InterviewScheduled", new
            {
                notification.Id,
                InterviewId = result.Id,
                JobTitle = result.JobTitle,
                Date = result.Date,
                Time = result.Time,
                Location = result.Location,
            });
        });

        return CreatedAtAction(null, result);
    }
}
