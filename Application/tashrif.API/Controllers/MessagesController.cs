using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/applications/{applicationId}/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessagesService _messagesService;

    public MessagesController(IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<MessageResponseDto>>> GetAll(
        long applicationId,
        [FromQuery] PaginationDto pagination)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _messagesService.GetByApplicationAsync(applicationId, userId, pagination);
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<MessageResponseDto>> Send(long applicationId, [FromBody] SendMessageDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _messagesService.SendAsync(applicationId, userId, dto.Body);
            return CreatedAtAction(nameof(GetAll), new { applicationId }, result);
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
