using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize(Policy = "Individual")]
[Route("api/qualifications")]
public class QualificationsController : ControllerBase
{
    private readonly IqualificationsService _service;

    public QualificationsController(IqualificationsService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<QualificationResponseDto>>> GetAll()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var items = await _service.GetAllByUserAsync(userId);
        var list = items.ToList();
        return Ok(new PaginationResultDto<QualificationResponseDto> { Items = list, Page = 1, Limit = list.Count, Total = list.Count });
    }

    [HttpPost]
    public async Task<ActionResult<QualificationResponseDto>> Create([FromBody] CreateQualificationDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetAll), null, result);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<QualificationResponseDto>> Update(long id, [FromBody] CreateQualificationDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.UpdateAsync(id, dto, userId);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _service.DeleteAsync(id, userId);
        return NoContent();
    }
}