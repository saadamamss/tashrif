using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize(Policy = "Individual")]
[Route("api/cvs")]
public class CvsController : ControllerBase
{
    private readonly IcvsService _service;

    public CvsController(IcvsService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<CvResponseDto>>> GetAll()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var items = await _service.GetAllByUserAsync(userId);
        var list = items.ToList();
        return Ok(new PaginationResultDto<CvResponseDto> { Items = list, Page = 1, Limit = list.Count, Total = list.Count });
    }

    [HttpPost]
    public async Task<ActionResult<CvResponseDto>> Create()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var dto = new CreateCvDto
        {
            File = Request.Form.Files.GetFile("file"),
        };
        var result = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetAll), null, result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _service.DeleteAsync(id, userId);
        return Ok(new { message = "تم حذف السيرة الذاتية بنجاح" });
    }
}
