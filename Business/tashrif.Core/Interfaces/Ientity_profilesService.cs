using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface Ientity_profilesService
{
    Task<EntityProfileResponseDto> GetByUserIdAsync(long userId);
    Task<EntityProfileResponseDto> UpdateAsync(long userId, UpdateEntityProfileDto dto);
    Task<EntityStatsDto> GetStatsAsync(long userId);
}
