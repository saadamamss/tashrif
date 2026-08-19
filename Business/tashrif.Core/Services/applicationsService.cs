using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class applicationsService(IUnitOfWork unitOfWork) : IapplicationsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PaginationResultDto<ApplicationResponseDto>> GetAllAsync(PaginationDto pagination, long userId, string? userType, string? status, string? search = null)
    {
        var query = await _unitOfWork.ApplicationsRepository.GetQueryable();
        query = query.Where(a => !a.IsDeleted);

        if (userType == "individual")
            query = query.Where(a => a.user_id == userId);
        else if (userType == "entity")
        {
            var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
            var entityJobIds = await jobsQuery.Where(j => j.entity_id == userId && !j.IsDeleted).Select(j => j.Id).ToListAsync();
            query = query.Where(a => entityJobIds.Contains(a.job_id));
        }

        if (!string.IsNullOrEmpty(status))
            query = query.Where(a => a.status == status);

        query = query.Include(a => a.user_Entity).Include(a => a.job_Entity);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(a => a.user_Entity.name.Contains(search));

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

        await PopulateJobsAsync(items);
        await PopulateApplicantDetailsAsync(items);

        return new PaginationResultDto<ApplicationResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    public async Task<ApplicationResponseDto> GetByIdAsync(long id, long userId)
    {
        var query = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await query
            .Include(a => a.user_Entity)
            .Include(a => a.job_Entity)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        if (app.user_id != userId && app.job_Entity.entity_id != userId)
            throw new UnauthorizedAccessException("غير مصرح لك بمشاهدة هذا الطلب");

        var (city, qualification) = await GetApplicantDetailsAsync(app.user_id);

        return new ApplicationResponseDto
        {
            Id = app.Id,
            JobId = app.job_id,
            UserId = app.user_id,
            UserName = app.user_Entity.name,
            UserGender = app.user_Entity.gender,
            UserCity = city,
            Qualification = string.IsNullOrEmpty(app.qualification) ? qualification : app.qualification,
            Status = app.status,
            CreatedAt = app.CreatedAt,
            Job = await BuildJobAsync(app.job_id),
        };
    }

    public async Task<ApplicationResponseDto> ApplyAsync(ApplyJobDto dto, long userId)
    {
        var existingQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var existing = await existingQuery
            .FirstOrDefaultAsync(a => a.job_id == dto.JobId && a.user_id == userId && !a.IsDeleted);
        if (existing != null)
            throw new InvalidOperationException("لقد تقدمت لهذه الوظيفة مسبقاً");

        var (city, qualification) = await GetApplicantDetailsAsync(userId);

        var app = new applications
        {
            job_id = dto.JobId,
            user_id = userId,
            qualification = string.IsNullOrEmpty(dto.Qualification) ? qualification : dto.Qualification,
            experience = dto.Experience ?? "",
            cover_letter = "",
            cv_id = null,
            status = "new",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ApplicationsRepository.AddAsync(app);
        await _unitOfWork.SaveChangesAsync();

        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == userId);
        var jobQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var job = await jobQuery.FirstOrDefaultAsync(j => j.Id == dto.JobId);

        return new ApplicationResponseDto
        {
            Id = app.Id,
            JobId = app.job_id,
            UserId = app.user_id,
            UserName = user?.name ?? "",
            UserGender = user?.gender,
            UserCity = city,
            Qualification = app.qualification,
            Status = app.status,
            CreatedAt = app.CreatedAt,
            Job = await BuildJobAsync(app.job_id),
        };
    }

    public async Task<ApplicationResponseDto> UpdateStatusAsync(long id, string status, long userId)
    {
        var validStatuses = new[] { "new", "shortlisted", "interview", "contract_sent", "accepted", "refused" };
        if (!validStatuses.Contains(status))
            throw new InvalidOperationException("حالة غير صالحة");

        var query = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await query
            .Include(a => a.user_Entity)
            .Include(a => a.job_Entity)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var job = await jobsQuery.FirstOrDefaultAsync(j => j.Id == app.job_id && !j.IsDeleted)
            ?? throw new KeyNotFoundException("الوظيفة غير موجودة");

        if (job.entity_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية تعديل هذا الطلب");

        app.status = status;
        app.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ApplicationsRepository.Update(app);
        await _unitOfWork.SaveChangesAsync();

        var (city, qualification) = await GetApplicantDetailsAsync(app.user_id);

        return new ApplicationResponseDto
        {
            Id = app.Id,
            JobId = app.job_id,
            UserId = app.user_id,
            UserName = app.user_Entity?.name ?? "",
            UserGender = app.user_Entity?.gender,
            UserCity = city,
            Qualification = string.IsNullOrEmpty(app.qualification) ? qualification : app.qualification,
            Status = app.status,
            CreatedAt = app.CreatedAt,
            Job = await BuildJobAsync(app.job_id),
        };
    }

    public async Task DeleteAsync(long id, long userId)
    {
        var query = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await query
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var job = await jobsQuery.FirstOrDefaultAsync(j => j.Id == app.job_id && !j.IsDeleted);

        var isJobOwner = job != null && job.entity_id == userId;
        if (!isJobOwner && app.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية حذف هذا الطلب");

        app.IsDeleted = true;
        app.DeletedTime = DateTime.UtcNow;
        _unitOfWork.ApplicationsRepository.Update(app);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task PopulateJobsAsync(IEnumerable<ApplicationResponseDto> items)
    {
        var jobIds = items.Select(a => a.JobId).Where(id => id > 0).Distinct().ToList();
        if (jobIds.Count == 0) return;

        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var jobsMap = await jobsQuery
            .Include(j => j.entity_Entity)
            .Where(j => jobIds.Contains(j.Id) && !j.IsDeleted)
            .ToDictionaryAsync(j => j.Id);

        var (benefits, conditions, responsibilities) = await GetJobListsAsync(jobIds);

        foreach (var app in items)
        {
            if (jobsMap.TryGetValue(app.JobId, out var entity))
            {
                var dto = MapJob(entity);
                dto.Benefits = benefits.GetValueOrDefault(app.JobId, new List<string>());
                dto.Conditions = conditions.GetValueOrDefault(app.JobId, new List<string>());
                dto.Responsibilities = responsibilities.GetValueOrDefault(app.JobId, new List<string>());
                app.Job = dto;
            }
        }
    }

    private async Task<JobResponseDto?> BuildJobAsync(long jobId)
    {
        if (jobId <= 0) return null;
        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var entity = await jobsQuery
            .Include(j => j.entity_Entity)
            .FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDeleted);
        if (entity == null) return null;

        var dto = MapJob(entity);
        var (benefits, conditions, responsibilities) = await GetJobListsAsync(new[] { jobId });
        dto.Benefits = benefits.GetValueOrDefault(jobId, new List<string>());
        dto.Conditions = conditions.GetValueOrDefault(jobId, new List<string>());
        dto.Responsibilities = responsibilities.GetValueOrDefault(jobId, new List<string>());
        return dto;
    }

    private async Task<(Dictionary<long, List<string>> benefits, Dictionary<long, List<string>> conditions, Dictionary<long, List<string>> responsibilities)> GetJobListsAsync(IEnumerable<long> jobIds)
    {
        var ids = jobIds.ToList();

        var benefitsQuery = await _unitOfWork.Job_benefitsRepository.GetQueryable();
        var benefits = await benefitsQuery
            .Where(b => ids.Contains(b.job_id) && !b.IsDeleted)
            .GroupBy(b => b.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(b => b.benefit_text).ToList());

        var conditionsQuery = await _unitOfWork.Job_conditionsRepository.GetQueryable();
        var conditions = await conditionsQuery
            .Where(c => ids.Contains(c.job_id) && !c.IsDeleted)
            .GroupBy(c => c.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(c => c.condition_text).ToList());

        var responsibilitiesQuery = await _unitOfWork.Job_responsibilitiesRepository.GetQueryable();
        var responsibilities = await responsibilitiesQuery
            .Where(r => ids.Contains(r.job_id) && !r.IsDeleted)
            .GroupBy(r => r.job_id)
            .ToDictionaryAsync(g => g.Key, g => g.Select(r => r.responsibility_text).ToList());

        return (benefits, conditions, responsibilities);
    }

    private async Task PopulateApplicantDetailsAsync(IEnumerable<ApplicationResponseDto> items)
    {
        var userIds = items.Select(a => a.UserId).Where(id => id > 0).Distinct().ToList();
        if (userIds.Count == 0) return;

        var profilesQuery = await _unitOfWork.Individual_profilesRepository.GetQueryable();
        var cityMap = await profilesQuery
            .Where(p => userIds.Contains(p.user_id) && !p.IsDeleted)
            .ToDictionaryAsync(p => p.user_id, p => p.city);

        var qualsQuery = await _unitOfWork.QualificationsRepository.GetQueryable();
        var quals = await qualsQuery
            .Where(q => userIds.Contains(q.user_id) && !q.IsDeleted)
            .ToListAsync();

        var qualsMap = quals
            .OrderByDescending(q => q.graduation_year)
            .GroupBy(q => q.user_id)
            .ToDictionary(g => g.Key, g => g.First().type);

        foreach (var app in items)
        {
            if (cityMap.TryGetValue(app.UserId, out var city))
                app.UserCity = city;
            if (string.IsNullOrEmpty(app.Qualification) && qualsMap.TryGetValue(app.UserId, out var qualification))
                app.Qualification = qualification;
        }
    }

    private async Task<(string city, string qualification)> GetApplicantDetailsAsync(long userId)
    {
        var profilesQuery = await _unitOfWork.Individual_profilesRepository.GetQueryable();
        var profile = await profilesQuery
            .FirstOrDefaultAsync(p => p.user_id == userId && !p.IsDeleted);

        var qualsQuery = await _unitOfWork.QualificationsRepository.GetQueryable();
        var primaryQualification = await qualsQuery
            .Where(q => q.user_id == userId && !q.IsDeleted)
            .OrderByDescending(q => q.graduation_year)
            .FirstOrDefaultAsync();

        return (profile?.city ?? "", primaryQualification?.type ?? "");
    }

    private static JobResponseDto MapJob(jobs j)
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
