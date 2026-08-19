using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IjobsService _jobsService;

    public JobsController(IjobsService jobsService)
    {
        _jobsService = jobsService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<JobResponseDto>>> GetAll(
        [FromQuery] PaginationDto pagination,
        [FromQuery] string? type,
        [FromQuery] string? gender,
        [FromQuery] string? search,
        [FromQuery] string? location,
        [FromQuery] long? entityId)
    {
        var userId = GetCurrentUserId();
        var result = await _jobsService.GetAllAsync(pagination, type, gender, search, location, entityId, userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobResponseDto>> GetById(long id)
    {
        var userId = GetCurrentUserId();
        var result = await _jobsService.GetJobByIdAsync(id, userId);
        return Ok(result);
    }

    [HttpGet("filter-options")]
    public async Task<ActionResult<JobFilterOptionsDto>> GetFilterOptions()
    {
        var result = await _jobsService.GetFilterOptionsAsync();
        return Ok(result);
    }

    [Authorize(Policy = "Entity")]
    [HttpGet("mine")]
    public async Task<ActionResult<PaginationResultDto<JobResponseDto>>> GetMyJobs(
        [FromQuery] PaginationDto pagination,
        [FromQuery] string? type,
        [FromQuery] string? status)
    {
        var entityId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _jobsService.GetMyJobsAsync(pagination, entityId, type, status);
        return Ok(result);
    }

    [Authorize(Policy = "Entity")]
    [HttpPost("publish")]
    public async Task<ActionResult<JobResponseDto>> Publish([FromBody] CreateJobDto dto)
    {
        var entityId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _jobsService.PublishAsync(dto, entityId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}/applications")]
    public async Task<ActionResult<PaginationResultDto<ApplicationResponseDto>>> GetApplications(
        long id,
        [FromQuery] PaginationDto pagination)
    {
        var result = await _jobsService.GetApplicationsAsync(id, pagination);
        return Ok(result);
    }

    private long? GetCurrentUserId()
    {
        var value = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(value, out var id) ? id : null;
    }
}
