using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface Icontact_personsService
{
    Task<IEnumerable<ContactPersonResponseDto>> GetAllByEntityAsync(long entityId);
    Task<ContactPersonResponseDto> CreateAsync(CreateContactPersonDto dto, long entityId);
}
