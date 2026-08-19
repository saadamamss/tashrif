using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface Iindividual_profilesService
{
    Task<IndividualProfileResponseDto> GetByUserIdAsync(long userId);
    Task<IndividualProfileResponseDto> UpdateAsync(long userId, UpdateIndividualProfileDto dto);
    Task<IndividualStatsDto> GetStatsAsync(long userId);
}
