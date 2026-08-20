using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly Iindividual_profilesService _individualService;
    private readonly Ientity_profilesService _entityService;

    public StatsController(
        Iindividual_profilesService individualService,
        Ientity_profilesService entityService)
    {
        _individualService = individualService;
        _entityService = entityService;
    }

    [HttpGet("api/stats/individual")]
    [Authorize(Policy = "Individual")]
    public async Task<ActionResult<IndividualStatsDto>> GetIndividualStats()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var stats = await _individualService.GetStatsAsync(userId);
        return Ok(stats);
    }

    [HttpGet("api/stats/entity")]
    [Authorize(Policy = "Entity")]
    public async Task<ActionResult<EntityStatsDto>> GetEntityStats()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var stats = await _entityService.GetStatsAsync(userId);
        return Ok(stats);
    }
}
