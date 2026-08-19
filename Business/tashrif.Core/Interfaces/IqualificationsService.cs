using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IqualificationsService
{
    Task<IEnumerable<QualificationResponseDto>> GetAllByUserAsync(long userId);
    Task<QualificationResponseDto> CreateAsync(CreateQualificationDto dto, long userId);
    Task<QualificationResponseDto> UpdateAsync(long id, CreateQualificationDto dto, long userId);
    Task DeleteAsync(long id, long userId);
}