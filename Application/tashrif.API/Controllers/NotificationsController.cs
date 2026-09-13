using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationsService _notificationsService;

    public NotificationsController(INotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<NotificationResponseDto>>> GetAll(
        [FromQuery] PaginationDto pagination,
        [FromQuery] bool? unreadOnly)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _notificationsService.GetByUserAsync(userId, pagination, unreadOnly);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var count = await _notificationsService.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    [Authorize]
    [HttpPost("{id}/read")]
    public async Task<ActionResult<NotificationResponseDto>> MarkRead(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _notificationsService.MarkReadAsync(id, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "الإشعار غير موجود" });
        }
    }

    [Authorize]
    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _notificationsService.MarkAllReadAsync(userId);
        return Ok(new { message = "تم تحديد الكل كمقروء" });
    }
}
