using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly Iindividual_profilesService _individualService;
    private readonly Ientity_profilesService _entityService;

    public ProfilesController(
        Iindividual_profilesService individualService,
        Ientity_profilesService entityService)
    {
        _individualService = individualService;
        _entityService = entityService;
    }

    [HttpGet("api/individuals/profile")]
    public async Task<ActionResult<IndividualProfileResponseDto>> GetIndividualProfile()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _individualService.GetByUserIdAsync(userId);
        return Ok(profile);
    }

    [HttpPut("api/individuals/profile")]
    public async Task<ActionResult<IndividualProfileResponseDto>> UpdateIndividualProfile(
        [FromBody] UpdateIndividualProfileDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _individualService.UpdateAsync(userId, dto);
        return Ok(profile);
    }

    [HttpGet("api/entities/profile")]
    public async Task<ActionResult<EntityProfileResponseDto>> GetEntityProfile()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _entityService.GetByUserIdAsync(userId);
        return Ok(profile);
    }

    [HttpPut("api/entities/profile")]
    public async Task<ActionResult<EntityProfileResponseDto>> UpdateEntityProfile(
        [FromBody] UpdateEntityProfileDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _entityService.UpdateAsync(userId, dto);
        return Ok(profile);
    }
}
