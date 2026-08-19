using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize(Policy = "Individual")]
[Route("api/bank-accounts")]
public class BankAccountsController : ControllerBase
{
    private readonly Ibank_accountsService _service;

    public BankAccountsController(Ibank_accountsService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<BankAccountResponseDto>>> GetAll()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var items = await _service.GetAllByUserAsync(userId);
        var list = items.ToList();
        return Ok(new PaginationResultDto<BankAccountResponseDto> { Items = list, Page = 1, Limit = list.Count, Total = list.Count });
    }

    [HttpPost]
    public async Task<ActionResult<BankAccountResponseDto>> Create([FromBody] CreateBankAccountDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetAll), null, result);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BankAccountResponseDto>> Update(long id, [FromBody] CreateBankAccountDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.UpdateAsync(id, dto, userId);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _service.DeleteAsync(id, userId);
        return Ok(new { message = "تم حذف الحساب البنكي بنجاح" });
    }
}
