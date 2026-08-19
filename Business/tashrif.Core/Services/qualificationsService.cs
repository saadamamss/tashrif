using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class qualificationsService(IUnitOfWork unitOfWork) : IqualificationsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<QualificationResponseDto>> GetAllByUserAsync(long userId)
    {
        var query = await _unitOfWork.QualificationsRepository.GetQueryable();
        return await query
            .Where(q => q.user_id == userId && !q.IsDeleted)
            .OrderByDescending(q => q.graduation_year)
            .Select(q => new QualificationResponseDto
            {
                Id = q.Id,
                Type = q.type,
                Specialization = q.specialization,
                Institution = q.institution,
                GraduationYear = q.graduation_year,
                Grade = q.grade,
            })
            .ToListAsync();
    }

    public async Task<QualificationResponseDto> CreateAsync(CreateQualificationDto dto, long userId)
    {
        var entity = new qualifications
        {
            user_id = userId,
            type = dto.QualificationType,
            specialization = dto.Specialization,
            institution = dto.Institution,
            graduation_year = dto.GraduationYear,
            grade = dto.Grade,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.QualificationsRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<QualificationResponseDto> UpdateAsync(long id, CreateQualificationDto dto, long userId)
    {
        var query = await _unitOfWork.QualificationsRepository.GetQueryable();
        var entity = await query
            .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted)
            ?? throw new KeyNotFoundException("المؤهل غير موجود");

        if (entity.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية تعديل هذا المؤهل");

        entity.type = dto.QualificationType;
        entity.specialization = dto.Specialization;
        entity.institution = dto.Institution;
        entity.graduation_year = dto.GraduationYear;
        entity.grade = dto.Grade;
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.QualificationsRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(long id, long userId)
    {
        var query = await _unitOfWork.QualificationsRepository.GetQueryable();
        var entity = await query
            .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted)
            ?? throw new KeyNotFoundException("المؤهل غير موجود");

        if (entity.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية حذف هذا المؤهل");

        entity.IsDeleted = true;
        entity.DeletedTime = DateTime.UtcNow;
        _unitOfWork.QualificationsRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    private static QualificationResponseDto Map(qualifications q)
    {
        return new QualificationResponseDto
        {
            Id = q.Id,
            Type = q.type,
            Specialization = q.specialization,
            Institution = q.institution,
            GraduationYear = q.graduation_year,
            Grade = q.grade,
        };
    }
}