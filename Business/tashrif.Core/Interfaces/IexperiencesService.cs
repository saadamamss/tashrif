using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IexperiencesService
{
    Task<IEnumerable<ExperienceResponseDto>> GetAllByUserAsync(long userId);
    Task<ExperienceResponseDto> CreateAsync(CreateExperienceDto dto, long userId);
    Task<ExperienceResponseDto> UpdateAsync(long id, CreateExperienceDto dto, long userId);
    Task DeleteAsync(long id, long userId);
}
