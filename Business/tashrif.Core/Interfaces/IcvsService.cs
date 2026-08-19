using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IcvsService
{
    Task<IEnumerable<CvResponseDto>> GetAllByUserAsync(long userId);
    Task<CvResponseDto> CreateAsync(CreateCvDto dto, long userId);
    Task DeleteAsync(long id, long userId);
}
