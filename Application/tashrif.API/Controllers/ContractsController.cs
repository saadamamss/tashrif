using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize]
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly IcontractsService _contractsService;

    public ContractsController(IcontractsService contractsService)
    {
        _contractsService = contractsService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<ContractResponseDto>>> GetAll([FromQuery] PaginationDto pagination)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userType = User.FindFirst(ClaimTypes.Role)?.Value;
        var result = await _contractsService.GetAllAsync(pagination, userId, userType);
        return Ok(result);
    }

    [Authorize(Policy = "Entity")]
    [HttpPost("send")]
    public async Task<ActionResult<ContractResponseDto>> Send([FromForm] SendContractDto dto)
    {
        var entityId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _contractsService.SendAsync(dto, entityId);
        return CreatedAtAction(null, result);
    }

    [HttpPost("{id}/sign")]
    public async Task<ActionResult<ContractResponseDto>> Sign(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _contractsService.SignAsync(id, userId);
        return Ok(result);
    }
}
