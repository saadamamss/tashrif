using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class contact_personsService(IUnitOfWork unitOfWork) : Icontact_personsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ContactPersonResponseDto>> GetAllByEntityAsync(long entityId)
    {
        var profileQuery = await _unitOfWork.Entity_profilesRepository.GetQueryable();
        var profile = await profileQuery.FirstOrDefaultAsync(p => p.user_id == entityId && !p.IsDeleted);

        if (profile == null) return Enumerable.Empty<ContactPersonResponseDto>();

        var query = await _unitOfWork.Contact_personsRepository.GetQueryable();
        return await query
            .Where(c => c.entity_id == profile.Id && !c.IsDeleted)
            .Select(c => new ContactPersonResponseDto
            {
                Id = c.Id,
                Name = c.name,
                Email = c.email,
                Phone = c.phone,
                Role = c.role,
                Nationality = c.nationality,
            })
            .ToListAsync();
    }

    public async Task<ContactPersonResponseDto> CreateAsync(CreateContactPersonDto dto, long entityId)
    {
        var profileQuery = await _unitOfWork.Entity_profilesRepository.GetQueryable();
        var profile = await profileQuery.FirstOrDefaultAsync(p => p.user_id == entityId && !p.IsDeleted)
            ?? throw new KeyNotFoundException("حساب الجهة غير مكتمل");

        var entity = new contact_persons
        {
            entity_id = profile.Id,
            name = dto.Name,
            email = dto.Email,
            phone = dto.Phone,
            role = dto.Role,
            nationality = dto.Nationality,
            is_primary = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.Contact_personsRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new ContactPersonResponseDto
        {
            Id = entity.Id,
            Name = entity.name,
            Email = entity.email,
            Phone = entity.phone,
            Role = entity.role,
            Nationality = entity.nationality,
        };
    }
}
