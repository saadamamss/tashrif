using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class jobsService(IUnitOfWork unitOfWork) : IjobsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PaginationResultDto<JobResponseDto>> GetAllAsync(PaginationDto pagination, string? type, string? gender, string? search, string? location = null, long? entityId = null, long? userId = null)
    {
        var query = await _unitOfWork.JobsRepository.GetQueryable();
        query = query.Where(j => !j.IsDeleted && j.status == "active")
            .Include(j => j.entity_Entity);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(j => j.work_type == type);
        if (!string.IsNullOrEmpty(gender))
            query = query.Where(j => j.gender == gender);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(j => j.title.Contains(search) || j.description.Contains(search));
        if (!string.IsNullOrEmpty(location))
            query = query.Where(j => j.location == location);
        if (entityId.HasValue)
            query = query.Where(j => j.entity_id == entityId.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .Select(j => new JobResponseDto
            {
                Id = j.Id,
                EntityId = j.entity_id,
                EntityName = j.entity_Entity.name,
                EntityLogo = j.entity_Entity.avatar_url,
                Title = j.title,
                Description = j.description,
                Location = j.location,
                Type = j.work_type,
                Target = j.target,
                Vacancies = j.vacancies,
                Qualification = j.qualification,
                Salary = j.salary_text ?? $"{j.salary_min} - {j.salary_max}",
                Gender = j.gender,
                Hours = j.hours,
                Duration = j.duration,
                Status = j.status,
                PublishDate = j.publish_date,
                EndDate = j.end_date,
                CreatedAt = j.CreatedAt,
            })
            .ToListAsync();

        var jobIds = items.Select(j => j.Id).ToList();

        var benefitsQuery = await _unitOfWork.Job_benefitsRepository.GetQueryable();
        var benefitsByJob = await benefitsQuery
            .Where(b => jobIds.Contains(b.job_id) && !b.IsDeleted)
            .GroupBy(b => b.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(b => b.benefit_text).ToList());

        var conditionsQuery = await _unitOfWork.Job_conditionsRepository.GetQueryable();
        var conditionsByJob = await conditionsQuery
            .Where(c => jobIds.Contains(c.job_id) && !c.IsDeleted)
            .GroupBy(c => c.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(c => c.condition_text).ToList());

        var responsibilitiesQuery = await _unitOfWork.Job_responsibilitiesRepository.GetQueryable();
        var responsibilitiesByJob = await responsibilitiesQuery
            .Where(r => jobIds.Contains(r.job_id) && !r.IsDeleted)
            .GroupBy(r => r.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(r => r.responsibility_text).ToList());

        var applicationsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var applicantsByJob = await applicationsQuery
            .Where(a => jobIds.Contains(a.job_id) && !a.IsDeleted)
            .GroupBy(a => a.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        foreach (var job in items)
        {
            job.Benefits = benefitsByJob.GetValueOrDefault(job.Id, new List<string>());
            job.Conditions = conditionsByJob.GetValueOrDefault(job.Id, new List<string>());
            job.Responsibilities = responsibilitiesByJob.GetValueOrDefault(job.Id, new List<string>());
            job.ApplicantCount = applicantsByJob.GetValueOrDefault(job.Id, 0);
        }

        if (userId.HasValue)
        {
            var appliedQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
            var appliedJobIds = (await appliedQuery
                .Where(a => a.user_id == userId.Value && jobIds.Contains(a.job_id) && !a.IsDeleted)
                .Select(a => a.job_id)
                .Distinct()
                .ToListAsync())
                .ToHashSet();

            foreach (var job in items)
            {
                job.IsApplied = appliedJobIds.Contains(job.Id);
            }
        }

        return new PaginationResultDto<JobResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    public async Task<PaginationResultDto<JobResponseDto>> GetMyJobsAsync(PaginationDto pagination, long entityUserId, string? type = null, string? status = null)
    {
        var query = await _unitOfWork.JobsRepository.GetQueryable();
        query = query.Where(j => j.entity_id == entityUserId && !j.IsDeleted)
            .Include(j => j.entity_Entity);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(j => j.work_type == type);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(j => j.status == status);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .Select(j => new JobResponseDto
            {
                Id = j.Id,
                EntityId = j.entity_id,
                EntityName = j.entity_Entity.name,
                EntityLogo = j.entity_Entity.avatar_url,
                Title = j.title,
                Description = j.description,
                Location = j.location,
                Type = j.work_type,
                Target = j.target,
                Vacancies = j.vacancies,
                Qualification = j.qualification,
                Salary = j.salary_text ?? $"{j.salary_min} - {j.salary_max}",
                Gender = j.gender,
                Hours = j.hours,
                Duration = j.duration,
                Status = j.status,
                PublishDate = j.publish_date,
                EndDate = j.end_date,
                CreatedAt = j.CreatedAt,
            })
            .ToListAsync();

        var jobIds = items.Select(j => j.Id).ToList();

        var benefitsQuery = await _unitOfWork.Job_benefitsRepository.GetQueryable();
        var benefitsByJob = await benefitsQuery
            .Where(b => jobIds.Contains(b.job_id) && !b.IsDeleted)
            .GroupBy(b => b.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(b => b.benefit_text).ToList());

        var conditionsQuery = await _unitOfWork.Job_conditionsRepository.GetQueryable();
        var conditionsByJob = await conditionsQuery
            .Where(c => jobIds.Contains(c.job_id) && !c.IsDeleted)
            .GroupBy(c => c.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(c => c.condition_text).ToList());

        var responsibilitiesQuery = await _unitOfWork.Job_responsibilitiesRepository.GetQueryable();
        var responsibilitiesByJob = await responsibilitiesQuery
            .Where(r => jobIds.Contains(r.job_id) && !r.IsDeleted)
            .GroupBy(r => r.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(r => r.responsibility_text).ToList());

        var applicationsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var applicantsByJob = await applicationsQuery
            .Where(a => jobIds.Contains(a.job_id) && !a.IsDeleted)
            .GroupBy(a => a.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        foreach (var job in items)
        {
            job.Benefits = benefitsByJob.GetValueOrDefault(job.Id, new List<string>());
            job.Conditions = conditionsByJob.GetValueOrDefault(job.Id, new List<string>());
            job.Responsibilities = responsibilitiesByJob.GetValueOrDefault(job.Id, new List<string>());
            job.ApplicantCount = applicantsByJob.GetValueOrDefault(job.Id, 0);
        }

        return new PaginationResultDto<JobResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    public async Task<JobResponseDto> GetJobByIdAsync(long id, long? userId = null)
    {
        var query = await _unitOfWork.JobsRepository.GetQueryable();
        var job = await query
            .Include(j => j.entity_Entity)
            .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted)
            ?? throw new KeyNotFoundException("الوظيفة غير موجودة");

        var dto = MapJobToDto(job);

        var benefitsQuery = await _unitOfWork.Job_benefitsRepository.GetQueryable();
        dto.Benefits = await benefitsQuery
            .Where(b => b.job_id == id && !b.IsDeleted)
            .Select(b => b.benefit_text)
            .ToListAsync();

        var conditionsQuery = await _unitOfWork.Job_conditionsRepository.GetQueryable();
        dto.Conditions = await conditionsQuery
            .Where(c => c.job_id == id && !c.IsDeleted)
            .Select(c => c.condition_text)
            .ToListAsync();

        var responsibilitiesQuery = await _unitOfWork.Job_responsibilitiesRepository.GetQueryable();
        dto.Responsibilities = await responsibilitiesQuery
            .Where(r => r.job_id == id && !r.IsDeleted)
            .Select(r => r.responsibility_text)
            .ToListAsync();

        if (userId.HasValue)
        {
            var appliedQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
            dto.IsApplied = await appliedQuery
                .AnyAsync(a => a.user_id == userId.Value && a.job_id == id && !a.IsDeleted);
        }

        var countQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        dto.ApplicantCount = await countQuery.CountAsync(a => a.job_id == id && !a.IsDeleted);

        return dto;
    }

    public async Task<JobFilterOptionsDto> GetFilterOptionsAsync()
    {
        var query = await _unitOfWork.JobsRepository.GetQueryable();
        var active = query.Where(j => !j.IsDeleted && j.status == "active");

        var types = await active.Select(j => j.work_type).Distinct().ToListAsync();
        var locations = await active.Select(j => j.location).Distinct().ToListAsync();
        var genders = await active.Select(j => j.gender).Distinct().ToListAsync();

        return new JobFilterOptionsDto
        {
            Types = types.Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList(),
            Locations = locations.Where(l => !string.IsNullOrEmpty(l)).Distinct().ToList(),
            Genders = genders.Where(g => !string.IsNullOrEmpty(g)).Distinct().ToList(),
        };
    }

    public async Task<JobResponseDto> PublishAsync(CreateJobDto dto, long entityUserId)
    {
        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == entityUserId && u.type == "entity" && !u.IsDeleted)
            ?? throw new InvalidOperationException("حساب الجهة غير مكتمل");

        var job = new jobs
        {
            entity_id = user.Id,
            title = dto.Title,
            description = dto.Description,
            location = dto.Location,
            work_type = dto.Type,
            target = dto.Target,
            vacancies = dto.Vacancies > 0 ? dto.Vacancies : 1,
            qualification = dto.Qualification,
            salary_text = dto.Salary,
            status = "active",
            gender = dto.Gender,
            hours = dto.Hours,
            duration = dto.Duration,
            end_date = dto.EndDate,
            publish_date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.JobsRepository.AddAsync(job);
        await _unitOfWork.SaveChangesAsync();

        if (dto.Benefits != null)
        {
            foreach (var benefit in dto.Benefits)
            {
                if (!string.IsNullOrEmpty(benefit))
                {
                    await _unitOfWork.Job_benefitsRepository.AddAsync(new job_benefits
                    {
                        job_id = job.Id,
                        benefit_text = benefit,
                        sort_order = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    });
                }
            }
        }

        if (dto.Conditions != null)
        {
            foreach (var condition in dto.Conditions)
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    await _unitOfWork.Job_conditionsRepository.AddAsync(new job_conditions
                    {
                        job_id = job.Id,
                        condition_text = condition,
                        sort_order = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    });
                }
            }
        }

        if (dto.Responsibilities != null)
        {
            foreach (var responsibility in dto.Responsibilities)
            {
                if (!string.IsNullOrEmpty(responsibility))
                {
                    await _unitOfWork.Job_responsibilitiesRepository.AddAsync(new job_responsibilities
                    {
                        job_id = job.Id,
                        responsibility_text = responsibility,
                        sort_order = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    });
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return await GetJobByIdAsync(job.Id);
    }

    public async Task<PaginationResultDto<ApplicationResponseDto>> GetApplicationsAsync(long jobId, long entityUserId, PaginationDto pagination)
    {
        var jobQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var job = await jobQuery.FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDeleted)
            ?? throw new KeyNotFoundException("الوظيفة غير موجودة");

        if (job.entity_id != entityUserId)
            throw new UnauthorizedAccessException("لا تملك صلاحية الوصول");

        var query = await _unitOfWork.ApplicationsRepository.GetQueryable();
        query = query.Where(a => a.job_id == jobId && !a.IsDeleted)
            .Include(a => a.user_Entity);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .Select(a => new ApplicationResponseDto
            {
                Id = a.Id,
                JobId = a.job_id,
                UserId = a.user_id,
                UserName = a.user_Entity.name,
                UserGender = a.user_Entity.gender,
                UserCity = "",
                Qualification = a.qualification,
                Status = a.status,
                CreatedAt = a.CreatedAt,
            })
            .ToListAsync();

        return new PaginationResultDto<ApplicationResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    private static JobResponseDto MapJobToDto(jobs j)
    {
        return new JobResponseDto
        {
            Id = j.Id,
            EntityId = j.entity_id,
            EntityName = j.entity_Entity?.name ?? "",
            EntityLogo = j.entity_Entity?.avatar_url,
            Title = j.title,
            Description = j.description,
            Location = j.location,
            Type = j.work_type,
            Target = j.target,
            Vacancies = j.vacancies,
            Qualification = j.qualification,
            Salary = j.salary_text ?? $"{j.salary_min} - {j.salary_max}",
            Gender = j.gender,
            Hours = j.hours,
            Duration = j.duration,
            Status = j.status,
            PublishDate = j.publish_date,
            EndDate = j.end_date,
            CreatedAt = j.CreatedAt,
        };
    }
}
