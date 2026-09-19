using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tashrif.Data.DTOs.Analytics;
using tashrif.Data.Interfaces;

namespace tashrif.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController(IanalyticsService analyticsService) : ControllerBase
{
    private readonly IanalyticsService _analyticsService = analyticsService;

    [Authorize(Policy = "Entity")]
    [HttpGet("entity")]
    public async Task<ActionResult<EntityAnalyticsDto>> GetEntityAnalytics()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _analyticsService.GetEntityAnalyticsAsync(userId);
        return Ok(result);
    }
}
