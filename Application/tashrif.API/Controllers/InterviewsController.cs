using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tashrif.API.Services;
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

    public InterviewsController(IinterviewsService interviewsService, IEmailService emailService, IBackgroundTaskQueue taskQueue)
    {
        _interviewsService = interviewsService;
        _emailService = emailService;
        _taskQueue = taskQueue;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<InterviewResponseDto>>> GetAll([FromQuery] PaginationDto pagination)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userType = User.FindFirst(ClaimTypes.Role)?.Value;
        var result = await _interviewsService.GetAllAsync(pagination, userId, userType);
        return Ok(result);
    }

    [Authorize(Policy = "Entity")]
    [HttpPost("schedule")]
    public async Task<ActionResult<InterviewResponseDto>> Schedule([FromBody] ScheduleInterviewDto dto)
    {
        var entityId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _interviewsService.ScheduleAsync(dto, entityId);

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
