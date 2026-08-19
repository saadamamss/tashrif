using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize]
[Route("api/interviews")]
public class InterviewsController : ControllerBase
{
    private readonly IinterviewsService _interviewsService;

    public InterviewsController(IinterviewsService interviewsService)
    {
        _interviewsService = interviewsService;
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
        return CreatedAtAction(null, result);
    }
}
