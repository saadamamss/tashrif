using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize(Policy = "Entity")]
[Route("api/contact-persons")]
public class ContactPersonsController : ControllerBase
{
    private readonly Icontact_personsService _service;

    public ContactPersonsController(Icontact_personsService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<ContactPersonResponseDto>>> GetAll()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var items = await _service.GetAllByEntityAsync(userId);
        var list = items.ToList();
        return Ok(new PaginationResultDto<ContactPersonResponseDto> { Items = list, Page = 1, Limit = list.Count, Total = list.Count });
    }

    [HttpPost]
    public async Task<ActionResult<ContactPersonResponseDto>> Create([FromBody] CreateContactPersonDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetAll), null, result);
    }
}
