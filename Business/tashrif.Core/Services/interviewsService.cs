using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class interviewsService(IUnitOfWork unitOfWork, IstatusHistoryService statusHistoryService) : IinterviewsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IstatusHistoryService _statusHistory = statusHistoryService;

    public async Task<PaginationResultDto<InterviewResponseDto>> GetAllAsync(PaginationDto pagination, long userId, string? userType, long? applicationId = null)
    {
        var query = await _unitOfWork.InterviewsRepository.GetQueryable();
        query = query.Where(i => !i.IsDeleted).Include(i => i.job_Entity)
            .Include(i => i.entity_Entity)
            .Include(i => i.user_Entity);

        if (userType == "individual")
            query = query.Where(i => i.user_id == userId);
        else if (userType == "entity")
            query = query.Where(i => i.entity_id == userId);

        // Narrow to a single application (details page). Ordering stays
        // CreatedAt DESC so Items[0] is always the latest interview.
        if (applicationId.HasValue)
            query = query.Where(i => i.application_id == applicationId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .Select(i => new InterviewResponseDto
            {
                Id = i.Id,
                ApplicationId = i.application_id,
                JobId = i.job_id,
                UserId = i.user_id,
                EntityId = i.entity_id,
                UserName = i.user_Entity.name,
                UserAvatar = i.user_Entity.avatar_url,
                EntityName = i.entity_Entity.name,
                EntityLogo = i.entity_Entity.avatar_url,
                Method = i.method,
                Date = i.interview_date,
                Time = i.interview_time.ToString() ?? "",
                Location = i.location,
                Link = i.link,
                Notes = i.notes,
                Status = i.status,
                Attendance = i.attendance,
                CreatedAt = i.CreatedAt,
            })
            .ToListAsync();

        return new PaginationResultDto<InterviewResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    public async Task<InterviewResponseDto> ScheduleAsync(ScheduleInterviewDto dto, long entityId)
    {
        var appQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await appQuery.FirstOrDefaultAsync(a => a.Id == dto.ApplicationId && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var job = await jobsQuery.FirstOrDefaultAsync(j => j.Id == app.job_id && !j.IsDeleted)
            ?? throw new KeyNotFoundException("الوظيفة غير موجودة");

        if (job.entity_id != entityId)
            throw new UnauthorizedAccessException("لا تملك صلاحية جدولة مقابلة لهذا الطلب");

        var usersQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var applicant = await usersQuery.FirstOrDefaultAsync(u => u.Id == app.user_id && !u.IsDeleted);
        var entityUser = await usersQuery.FirstOrDefaultAsync(u => u.Id == entityId && !u.IsDeleted);

        var interview = new interviews
        {
            application_id = dto.ApplicationId,
            job_id = app.job_id,
            user_id = app.user_id,
            entity_id = entityId,
            method = dto.Method,
            interview_date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
            interview_time = dto.Time,
            location = dto.Location ?? "",
            link = dto.Link ?? "",
            notes = dto.Notes ?? "",
            status = "scheduled",
            attendance = "pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.InterviewsRepository.AddAsync(interview);

        var oldStatus = app.status;
        app.status = "interview";
        app.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ApplicationsRepository.Update(app);
        await _unitOfWork.SaveChangesAsync();

        // Record status history
        await _statusHistory.RecordAsync(dto.ApplicationId, oldStatus, "interview", entityId);

        return new InterviewResponseDto
        {
            Id = interview.Id,
            ApplicationId = interview.application_id,
            JobId = interview.job_id,
            UserId = interview.user_id,
            EntityId = interview.entity_id,
            UserName = applicant?.name ?? "",
            UserEmail = applicant?.email ?? null,
            UserAvatar = applicant?.avatar_url ?? "",
            EntityName = entityUser?.name ?? "",
            EntityLogo = entityUser?.avatar_url ?? "",
            JobTitle = job.title,
            Method = interview.method,
            Date = interview.interview_date,
            Time = interview.interview_time.ToString() ?? "",
            Location = interview.location,
            Link = interview.link,
            Notes = interview.notes,
            Status = interview.status,
            Attendance = interview.attendance,
            CreatedAt = interview.CreatedAt,
        };
    }
}
