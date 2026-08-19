using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class experiencesService(IUnitOfWork unitOfWork) : IexperiencesService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ExperienceResponseDto>> GetAllByUserAsync(long userId)
    {
        var query = await _unitOfWork.ExperiencesRepository.GetQueryable();
        return await query
            .Where(e => e.user_id == userId && !e.IsDeleted)
            .OrderByDescending(e => e.start_date)
            .Select(e => new ExperienceResponseDto
            {
                Id = e.Id,
                UserId = e.user_id,
                JobTitle = e.job_title,
                Employer = e.employer,
                Duration = e.duration,
                Location = e.location,
                IsCurrent = e.is_current,
                StartDate = e.start_date,
                EndDate = e.end_date,
                CreatedAt = e.CreatedAt,
            })
            .ToListAsync();
    }

    public async Task<ExperienceResponseDto> CreateAsync(CreateExperienceDto dto, long userId)
    {
        var entity = new experiences
        {
            user_id = userId,
            job_title = dto.JobTitle,
            employer = dto.Employer,
            duration = dto.Duration,
            location = dto.Location,
            is_current = dto.IsCurrent,
            start_date = dto.StartDate.HasValue ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc) : null,
            end_date = dto.IsCurrent ? null : dto.EndDate.HasValue ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.ExperiencesRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<ExperienceResponseDto> UpdateAsync(long id, CreateExperienceDto dto, long userId)
    {
        var query = await _unitOfWork.ExperiencesRepository.GetQueryable();
        var entity = await query
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted)
            ?? throw new KeyNotFoundException("الخبرة غير موجودة");

        if (entity.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية تعديل هذه الخبرة");

        entity.job_title = dto.JobTitle;
        entity.employer = dto.Employer;
        entity.duration = dto.Duration;
        entity.location = dto.Location;
        entity.is_current = dto.IsCurrent;
        entity.start_date = dto.StartDate.HasValue ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc) : null;
        entity.end_date = dto.IsCurrent ? null : dto.EndDate.HasValue ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) : null;
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ExperiencesRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(long id, long userId)
    {
        var query = await _unitOfWork.ExperiencesRepository.GetQueryable();
        var entity = await query
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted)
            ?? throw new KeyNotFoundException("الخبرة غير موجودة");

        if (entity.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية حذف هذه الخبرة");

        entity.IsDeleted = true;
        entity.DeletedTime = DateTime.UtcNow;
        _unitOfWork.ExperiencesRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    private static ExperienceResponseDto Map(experiences e)
    {
        return new ExperienceResponseDto
        {
            Id = e.Id,
            UserId = e.user_id,
            JobTitle = e.job_title,
            Employer = e.employer,
            Duration = e.duration,
            Location = e.location,
            IsCurrent = e.is_current,
            StartDate = e.start_date,
            EndDate = e.end_date,
            CreatedAt = e.CreatedAt,
        };
    }
}
