using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class applicationsService(IUnitOfWork unitOfWork, IstatusHistoryService statusHistoryService) : IapplicationsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IstatusHistoryService _statusHistory = statusHistoryService;

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
                Experience = a.experience ?? "",
                CvId = a.cv_id,
                Status = a.status,
                CreatedAt = a.CreatedAt,
            })
            .ToListAsync();

        await PopulateJobsAsync(items);
        await PopulateApplicantDetailsAsync(items);
        await PopulateCvsAsync(items);

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
        var (cvFileName, cvFilePath) = await GetCvInfoAsync(app.cv_id);

        return new ApplicationResponseDto
        {
            Id = app.Id,
            JobId = app.job_id,
            UserId = app.user_id,
            UserName = app.user_Entity.name,
            UserGender = app.user_Entity.gender,
            UserCity = city,
            Qualification = string.IsNullOrEmpty(app.qualification) ? qualification : app.qualification,
            Experience = app.experience ?? "",
            CvId = app.cv_id,
            CvFileName = cvFileName,
            CvFilePath = cvFilePath,
            Status = app.status,
            CreatedAt = app.CreatedAt,
            Job = await BuildJobAsync(app.job_id),
        };
    }

    public async Task<ApplicationResponseDto> ApplyAsync(ApplyJobDto dto, long userId)
    {
        // Load the job once, up front: it must exist (not soft-deleted) and be open for applications.
        // Without the status guard, apply-to-closed jobs succeeds (QA bug 2026-10-01, see ISSUES.md).
        var applyJobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var applyJob = await applyJobsQuery
            .FirstOrDefaultAsync(j => j.Id == dto.JobId && !j.IsDeleted)
            ?? throw new KeyNotFoundException("الوظيفة غير موجودة");
        if (applyJob.status != "active")
            throw new InvalidOperationException("لا يمكن التقديم على وظيفة غير مفتوحة");

        var existingQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var existing = await existingQuery
            .FirstOrDefaultAsync(a => a.job_id == dto.JobId && a.user_id == userId && !a.IsDeleted && a.status != "withdrawn");
        if (existing != null)
            throw new InvalidOperationException("لقد تقدمت لهذه الوظيفة مسبقاً");

        // Resolve qualification by ID
        var qualsQuery = await _unitOfWork.QualificationsRepository.GetQueryable();
        var qual = await qualsQuery
            .FirstOrDefaultAsync(q => q.Id == dto.QualificationId && q.user_id == userId && !q.IsDeleted);
        if (qual == null)
            throw new InvalidOperationException("المؤهل المحدد غير موجود");

        // Resolve CV by ID — required since the required-CV change: must belong to the applicant.
        var cvsQuery = await _unitOfWork.CvsRepository.GetQueryable();
        var cv = await cvsQuery
            .FirstOrDefaultAsync(c => c.Id == dto.CvId && c.user_id == userId && !c.IsDeleted);
        if (cv == null)
            throw new InvalidOperationException("السيرة الذاتية المحددة غير موجودة");

        var (city, _) = await GetApplicantDetailsAsync(userId);

        var app = new applications
        {
            job_id = dto.JobId,
            user_id = userId,
            qualification = qual.type,
            experience = dto.Experience ?? "",
            cover_letter = "",
            cv_id = dto.CvId,
            status = "new",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ApplicationsRepository.AddAsync(app);
        await _unitOfWork.SaveChangesAsync();

        // Record initial status history
        await _statusHistory.RecordAsync(app.Id, null, "new", userId);

        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == userId);

        return new ApplicationResponseDto
        {
            Id = app.Id,
            JobId = app.job_id,
            UserId = app.user_id,
            UserName = user?.name ?? "",
            UserGender = user?.gender,
            UserCity = city,
            Qualification = app.qualification,
            Experience = app.experience ?? "",
            CvId = app.cv_id,
            CvFileName = cv.file_name,
            CvFilePath = cv.file_path,
            Status = app.status,
            CreatedAt = app.CreatedAt,
            Job = await BuildJobAsync(applyJob),
        };
    }

    public async Task<ApplicationResponseDto> UpdateStatusAsync(long id, string status, long userId)
    {
        var validStatuses = new[] { "new", "shortlisted", "interview", "contract_sent", "accepted", "refused", "withdrawn" };
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

        var oldStatus = app.status;
        app.status = status;
        app.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ApplicationsRepository.Update(app);
        await _unitOfWork.SaveChangesAsync();

        // Record status history
        await _statusHistory.RecordAsync(app.Id, oldStatus, status, userId);

        var (city, qualification) = await GetApplicantDetailsAsync(app.user_id);
        var (statusCvFileName, statusCvFilePath) = await GetCvInfoAsync(app.cv_id);

        return new ApplicationResponseDto
        {
            Id = app.Id,
            JobId = app.job_id,
            UserId = app.user_id,
            UserName = app.user_Entity?.name ?? "",
            UserGender = app.user_Entity?.gender,
            UserCity = city,
            Qualification = string.IsNullOrEmpty(app.qualification) ? qualification : app.qualification,
            Experience = app.experience ?? "",
            CvId = app.cv_id,
            CvFileName = statusCvFileName,
            CvFilePath = statusCvFilePath,
            Status = app.status,
            CreatedAt = app.CreatedAt,
            Job = await BuildJobAsync(app.job_id),
        };
    }

    public async Task<ApplicationResponseDto> WithdrawAsync(long id, long userId)
    {
        var query = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await query
            .Include(a => a.user_Entity)
            .Include(a => a.job_Entity)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        // Only the application owner can withdraw
        if (app.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية سحب هذا الطلب");

        // Cannot withdraw if already at contract stage or beyond
        var withdrawableStatuses = new[] { "new", "shortlisted", "interview" };
        if (!withdrawableStatuses.Contains(app.status))
            throw new InvalidOperationException("لا يمكن سحب الطلب بعد مرحلة العقد");

        var oldStatus = app.status;
        app.status = "withdrawn";
        app.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ApplicationsRepository.Update(app);
        await _unitOfWork.SaveChangesAsync();

        // Record status history
        await _statusHistory.RecordAsync(app.Id, oldStatus, "withdrawn", userId);

        var (city, qualification) = await GetApplicantDetailsAsync(app.user_id);
        var (withdrawCvFileName, withdrawCvFilePath) = await GetCvInfoAsync(app.cv_id);

        return new ApplicationResponseDto
        {
            Id = app.Id,
            JobId = app.job_id,
            UserId = app.user_id,
            UserName = app.user_Entity?.name ?? "",
            UserGender = app.user_Entity?.gender,
            UserCity = city,
            Qualification = string.IsNullOrEmpty(app.qualification) ? qualification : app.qualification,
            Experience = app.experience ?? "",
            CvId = app.cv_id,
            CvFileName = withdrawCvFileName,
            CvFilePath = withdrawCvFilePath,
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

    public async Task<List<BulkActionResultDto>> BulkActionAsync(List<long> applicationIds, string action, long entityId)
    {
        if (applicationIds.Count == 0)
            return new List<BulkActionResultDto>();

        var targetStatus = action switch
        {
            "shortlist" => "shortlisted",
            "refuse" => "refused",
            "restore" => "new",
            _ => throw new InvalidOperationException("إجراء غير صالح")
        };

        // Step 1: Load applications with job info for ownership check (AsNoTracking)
        var query = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var apps = await query
            .Include(a => a.job_Entity)
            .Where(a => applicationIds.Contains(a.Id) && !a.IsDeleted)
            .ToListAsync();

        var foundIds = apps.Select(a => a.Id).ToHashSet();
        var now = DateTime.UtcNow;
        var results = new List<BulkActionResultDto>();
        var idsToUpdate = new List<long>();

        foreach (var id in applicationIds)
        {
            if (!foundIds.Contains(id))
            {
                results.Add(new BulkActionResultDto
                {
                    ApplicationId = id,
                    Success = false,
                    Error = "الطلب غير موجود"
                });
                continue;
            }

            var app = apps.First(a => a.Id == id);

            if (app.job_Entity == null || app.job_Entity.entity_id != entityId)
            {
                results.Add(new BulkActionResultDto
                {
                    ApplicationId = id,
                    Success = false,
                    Error = "لا تملك صلاحية تعديل هذا الطلب"
                });
                continue;
            }

            idsToUpdate.Add(id);
            results.Add(new BulkActionResultDto
            {
                ApplicationId = id,
                Success = true,
                OldStatus = app.status,
                NewStatus = targetStatus
            });
        }

        // Step 2: Batch update via ExecuteUpdateAsync — bypasses change tracker entirely
        // (avoids duplicate tracking when multiple apps share the same job)
        if (idsToUpdate.Count > 0)
        {
            var updateQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
            await updateQuery
                .Where(a => idsToUpdate.Contains(a.Id))
                .ExecuteUpdateAsync(a => a
                    .SetProperty(x => x.status, targetStatus)
                    .SetProperty(x => x.UpdatedAt, now));

            // Record status history for all successful applications in one save
            var historyEntries = results.Where(r => r.Success)
                .Select(r => (r.ApplicationId, r.OldStatus, NewStatus: targetStatus))
                .ToList();
            await _statusHistory.RecordBatchAsync(historyEntries, entityId);
        }

        return results;
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
        return await BuildJobAsync(entity);
    }

    private Task<JobResponseDto?> BuildJobAsync(jobs entity)
    {
        var dto = MapJob(entity);
        return BuildJobListsAsync(entity.Id, dto);
    }

    private async Task<JobResponseDto?> BuildJobListsAsync(long jobId, JobResponseDto dto)
    {
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

    private async Task PopulateCvsAsync(IEnumerable<ApplicationResponseDto> items)
    {
        var list = items.ToList();
        var cvIds = list.Select(a => a.CvId).Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (cvIds.Count == 0) return;

        var cvsQuery = await _unitOfWork.CvsRepository.GetQueryable();
        var cvMap = await cvsQuery
            .Where(c => cvIds.Contains(c.Id) && !c.IsDeleted)
            .ToDictionaryAsync(c => c.Id);

        foreach (var app in list)
        {
            if (app.CvId.HasValue && cvMap.TryGetValue(app.CvId.Value, out var cv))
            {
                app.CvFileName = cv.file_name;
                app.CvFilePath = cv.file_path;
            }
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

    private async Task<(string? fileName, string? filePath)> GetCvInfoAsync(long? cvId)
    {
        if (!cvId.HasValue) return (null, null);

        var cvsQuery = await _unitOfWork.CvsRepository.GetQueryable();
        var cv = await cvsQuery
            .FirstOrDefaultAsync(c => c.Id == cvId.Value && !c.IsDeleted);
        if (cv == null) return (null, null);
        return (cv.file_name, cv.file_path);
    }

    private static JobResponseDto MapJob(jobs j)
    {
        return new JobResponseDto
        {
            Id = j.Id,
            EntityId = j.entity_id,
            EntityName = j.entity_Entity?.name ?? "",
            EntityLogo = j.entity_Entity?.avatar_url,
            EntityEmail = j.entity_Entity?.email ?? null,
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
