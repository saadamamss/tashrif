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
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly IcontractsService _contractsService;
    private readonly IEmailService _emailService;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IAuditService _auditService;

    public ContractsController(IcontractsService contractsService, IEmailService emailService, IBackgroundTaskQueue taskQueue, IAuditService auditService)
    {
        _contractsService = contractsService;
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
    public async Task<ActionResult<PaginationResultDto<ContractResponseDto>>> GetAll([FromQuery] PaginationDto pagination, [FromQuery] long? applicationId)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userType = User.FindFirst(ClaimTypes.Role)?.Value;
        var result = await _contractsService.GetAllAsync(pagination, userId, userType, applicationId);
        return Ok(result);
    }

    [Authorize(Policy = "Entity")]
    [HttpPost("send")]
    public async Task<ActionResult<ContractResponseDto>> Send([FromForm] SendContractDto dto)
    {
        var entityId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _contractsService.SendAsync(dto, entityId);
        await TryLog(entityId, AuditActions.ContractSent, "contracts", result.Id, null, "sent");

        _taskQueue.Enqueue(async scope =>
        {
            var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
            var notificationHub = scope.ServiceProvider.GetRequiredService<INotificationHubService>();

            if (result.UserEmail is not null)
            {
                await _emailService.SendAsync(result.UserEmail, "تم إرسال عقد عمل إليك لتوقيعه", EmailTemplates.ContractSent(result.UserName!, result.JobTitle!));
            }

            var notification = await notificationsService.CreateAsync(
                result.UserId,
                "عقد جديد",
                $"تم إرسال عقد لوظيفة {result.JobTitle} من {result.EntityName}",
                "contract_sent",
                result.Id,
                "contract");

            await notificationHub.SendToUserAsync(result.UserId, "ContractSent", new
            {
                notification.Id,
                ContractId = result.Id,
                JobTitle = result.JobTitle,
                EntityName = result.EntityName,
            });
        });

        return CreatedAtAction(null, result);
    }

    [HttpPost("{id}/sign")]
    public async Task<ActionResult<ContractResponseDto>> Sign(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _contractsService.SignAsync(id, userId);
        await TryLog(userId, AuditActions.ContractSigned, "contracts", result.Id, "sent", "signed");

        _taskQueue.Enqueue(async scope =>
        {
            var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
            var notificationHub = scope.ServiceProvider.GetRequiredService<INotificationHubService>();

            if (result.EntityEmail is not null)
            {
                await _emailService.SendAsync(result.EntityEmail,$"تم توقيع عقد العمل الخاص بـ {result.UserName}", EmailTemplates.ContractSigned(result.EntityName!, result.UserName!, result.JobTitle!));
            }

            var notification = await notificationsService.CreateAsync(
                result.EntityId,
                "تم توقيع العقد",
                $"{result.UserName} وقّع العقد لوظيفة {result.JobTitle}",
                "contract_signed",
                result.Id,
                "contract");

            await notificationHub.SendToUserAsync(result.EntityId, "ContractSigned", new
            {
                notification.Id,
                ContractId = result.Id,
                JobTitle = result.JobTitle,
                UserName = result.UserName,
            });
        });

        return Ok(result);
    }
}
