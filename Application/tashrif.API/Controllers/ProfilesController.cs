using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tashrif.Data.Constants;

namespace tashrif.API.Controllers;

[ApiController]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly Iindividual_profilesService _individualService;
    private readonly Ientity_profilesService _entityService;
    private readonly IFileStorageService _fileStorage;
    private readonly IAuditService _auditService;

    public ProfilesController(
        Iindividual_profilesService individualService,
        Ientity_profilesService entityService,
        IFileStorageService fileStorage,
        IAuditService auditService)
    {
        _individualService = individualService;
        _entityService = entityService;
        _fileStorage = fileStorage;
        _auditService = auditService;
    }

    /// <summary>Audit writes must never fail the user's operation — log &amp; continue.</summary>
    private async Task TryLog(long userId, string action, string entityType, long entityId,
        string? oldValue = null, string? newValue = null)
    {
        try
        {
            await _auditService.LogAsync(userId, action, entityType, entityId, oldValue, newValue,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[audit] failed to write {action} for user {userId}: {ex.Message}");
        }
    }

    [HttpGet("api/individuals/profile")]
    [Authorize(Policy = "Individual")]
    public async Task<ActionResult<IndividualProfileResponseDto>> GetIndividualProfile()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _individualService.GetByUserIdAsync(userId);
        return Ok(profile);
    }

    [HttpPut("api/individuals/profile")]
    [Authorize(Policy = "Individual")]
    public async Task<ActionResult<IndividualProfileResponseDto>> UpdateIndividualProfile(
        [FromBody] UpdateIndividualProfileDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _individualService.UpdateAsync(userId, dto);
        // null/null old/new — profile diffs would be a separate privacy decision.
        await TryLog(userId, AuditActions.ProfileUpdated, "users", userId);
        return Ok(profile);
    }

    [HttpPut("api/individuals/profile/avatar")]
    [Authorize(Policy = "Individual")]
    public async Task<ActionResult<IndividualProfileResponseDto>> UpdateIndividualAvatar(
        IFormFile file)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var avatarUrl = await _fileStorage.SaveFileAsync(file, "avatars");
        var profile = await _individualService.UpdateAsync(userId, new UpdateIndividualProfileDto { AvatarUrl = avatarUrl });
        return Ok(profile);
    }

    [HttpGet("api/entities/profile")]
    [Authorize(Policy = "Entity")]
    public async Task<ActionResult<EntityProfileResponseDto>> GetEntityProfile()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _entityService.GetByUserIdAsync(userId);
        return Ok(profile);
    }

    [HttpPut("api/entities/profile")]
    [Authorize(Policy = "Entity")]
    public async Task<ActionResult<EntityProfileResponseDto>> UpdateEntityProfile(
        [FromBody] UpdateEntityProfileDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _entityService.UpdateAsync(userId, dto);
        // null/null old/new — profile diffs would be a separate privacy decision.
        await TryLog(userId, AuditActions.ProfileUpdated, "users", userId);
        return Ok(profile);
    }

    [HttpPut("api/entities/profile/logo")]
    [Authorize(Policy = "Entity")]
    public async Task<ActionResult<EntityProfileResponseDto>> UpdateEntityLogo(
        IFormFile file)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var logoUrl = await _fileStorage.SaveFileAsync(file, "logos");
        var profile = await _entityService.UpdateAsync(userId, new UpdateEntityProfileDto { LogoUrl = logoUrl });
        return Ok(profile);
    }
}
