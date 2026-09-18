using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using tashrif.Data.DTOs.Admin;
using tashrif.Data.DTOs.Auth;

namespace tashrif.Core;

public class adminService(IUnitOfWork unitOfWork, IFileStorageService fileStorage) : IAdminService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorage = fileStorage;

    public async Task<AdminStatsDto> GetStatsAsync()
    {
        // D6: sequential awaits — one scoped DbContext is not thread-safe,
        // so parallel CountAsync() on the same context instance would throw.
        var usersQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var jobsQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var applicationsQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var contractsQuery = await _unitOfWork.ContractsRepository.GetQueryable();

        var stats = new AdminStatsDto
        {
            TotalUsers = await usersQuery.CountAsync(),
            TotalIndividuals = await usersQuery.CountAsync(u => u.type == "individual"),
            TotalEntities = await usersQuery.CountAsync(u => u.type == "entity"),
            TotalAdmins = await usersQuery.CountAsync(u => u.type == "admin"),
            TotalJobs = await jobsQuery.CountAsync(),
            ActiveJobs = await jobsQuery.CountAsync(j => j.status == "active"),
            TotalApplications = await applicationsQuery.CountAsync(),
            TotalContracts = await contractsQuery.CountAsync(),
        };

        return stats;
    }

    public async Task<UserDto> UpdateProfileAsync(long adminId, UpdateAdminProfileDto dto)
    {
        // GetByIdAsync uses FindAsync (tracked), unlike GetQueryable() (AsNoTracking)
        var admin = await _unitOfWork.UsersRepository.GetByIdAsync(adminId)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        if (admin.type != "admin")
            throw new UnauthorizedAccessException("لا تملك صلاحية الوصول");

        // Email uniqueness guard (error text mirrors AuthService.RegisterAsync)
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var emailQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var emailTaken = await emailQuery.AnyAsync(u =>
            u.Id != adminId && u.email.ToLower() == normalizedEmail);

        if (emailTaken)
            throw new BadHttpRequestException("البريد الإلكتروني مستخدم مسبقا", 400);

        admin.name = dto.Name.Trim();
        admin.email = normalizedEmail;
        admin.phone = dto.Phone;
        admin.gender = dto.Gender;
        admin.nationality = dto.Nationality;
        admin.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.UsersRepository.Update(admin);
        await _unitOfWork.SaveChangesAsync();

        return new UserDto
        {
            Id = admin.Id,
            Name = admin.name,
            Email = admin.email,
            Type = admin.type,
            Phone = admin.phone,
            NationalId = admin.national_id,
            Gender = admin.gender,
            Nationality = admin.nationality,
            AvatarUrl = admin.avatar_url,
        };
    }

    public async Task<UserDto> UpdateAvatarAsync(long adminId, string avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(avatarUrl))
            throw new BadHttpRequestException("يرجى اختيار صورة", 400);

        var admin = await _unitOfWork.UsersRepository.GetByIdAsync(adminId)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        if (admin.type != "admin")
            throw new UnauthorizedAccessException("لا تملك صلاحية الوصول");

        admin.avatar_url = avatarUrl;
        admin.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.UsersRepository.Update(admin);
        await _unitOfWork.SaveChangesAsync();

        return new UserDto
        {
            Id = admin.Id,
            Name = admin.name,
            Email = admin.email,
            Type = admin.type,
            Phone = admin.phone,
            NationalId = admin.national_id,
            Gender = admin.gender,
            Nationality = admin.nationality,
            AvatarUrl = admin.avatar_url,
        };
    }

    public async Task<PaginationResultDto<AdminUserDto>> GetUsersAsync(AdminUserFilterDto filter)
    {
        // D10: IgnoreQueryFilters so the admin can see (and restore) deactivated users.
        var query = await _unitOfWork.UsersRepository.GetQueryable();
        query = query.IgnoreQueryFilters();

        if (!string.IsNullOrEmpty(filter.Type))
            query = query.Where(u => u.type == filter.Type);

        if (filter.Status == "active")
            query = query.Where(u => !u.IsDeleted);
        else if (filter.Status == "deactivated")
            query = query.Where(u => u.IsDeleted);

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(u =>
                u.name.Contains(search) ||
                u.email.Contains(search) ||
                u.national_id.Contains(search));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((filter.Page - 1) * filter.Limit)
            .Take(filter.Limit)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                Name = u.name,
                Email = u.email,
                NationalId = u.national_id,
                Phone = u.phone,
                Type = u.type,
                Gender = u.gender,
                Nationality = u.nationality,
                AvatarUrl = u.avatar_url,
                IsDeleted = u.IsDeleted,
                CreatedAt = u.CreatedAt,
            })
            .ToListAsync();

        return new PaginationResultDto<AdminUserDto>
        {
            Items = items,
            Page = filter.Page,
            Limit = filter.Limit,
            Total = total,
        };
    }

    public async Task<AdminUserDto?> GetUserByIdAsync(long id)
    {
        var query = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await query
            .IgnoreQueryFilters() // D10: admin can view deactivated users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        return new AdminUserDto
        {
            Id = user.Id,
            Name = user.name,
            Email = user.email,
            NationalId = user.national_id,
            Phone = user.phone,
            Type = user.type,
            Gender = user.gender,
            Nationality = user.nationality,
            AvatarUrl = user.avatar_url,
            IsDeleted = user.IsDeleted,
            CreatedAt = user.CreatedAt,
        };
    }

    public async Task DeactivateUserAsync(long id, long adminId)
    {
        if (id == adminId)
            throw new BadHttpRequestException("لا يمكن تعطيل حسابك الخاص", 400);

        var query = await _unitOfWork.UsersRepository.GetQueryable();
        var exists = await query
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Id == id);

        if (!exists)
            throw new KeyNotFoundException("المستخدم غير موجود");

        // D7: ExecuteUpdateAsync — GetQueryable() is AsNoTracking; load-modify-Update()
        // hits the duplicate-tracking issue (see known-issues.md).
        var now = DateTime.UtcNow;
        await query
            .IgnoreQueryFilters()
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(x => x.IsDeleted, true)
                .SetProperty(x => x.DeletedTime, now)
                .SetProperty(x => x.UpdatedAt, now));
    }

    public async Task ActivateUserAsync(long id)
    {
        var query = await _unitOfWork.UsersRepository.GetQueryable();
        var exists = await query
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Id == id);

        if (!exists)
            throw new KeyNotFoundException("المستخدم غير موجود");

        var now = DateTime.UtcNow;
        await query
            .IgnoreQueryFilters()
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(x => x.IsDeleted, false)
                .SetProperty(x => x.DeletedTime, (DateTime?)null)
                .SetProperty(x => x.UpdatedAt, now));
    }

    public async Task<PaginationResultDto<AdminAuditLogDto>> GetAuditLogsAsync(AdminAuditLogFilterDto filter)
    {
        // audit_logs has no navigation property to users (only user_id), so we
        // batch-fetch user names after paging — avoids a cross-repository join.
        var query = await _unitOfWork.Audit_logsRepository.GetQueryable();

        if (filter.UserId.HasValue)
            query = query.Where(l => l.user_id == filter.UserId.Value);
        if (!string.IsNullOrEmpty(filter.Action))
            query = query.Where(l => l.action == filter.Action);
        if (!string.IsNullOrEmpty(filter.EntityType))
            query = query.Where(l => l.entity_type == filter.EntityType);
        if (filter.From.HasValue)
        {
            var from = DateTime.SpecifyKind(filter.From.Value, DateTimeKind.Utc);
            query = query.Where(l => l.CreatedAt >= from);
        }
        if (filter.To.HasValue)
        {
            var to = DateTime.SpecifyKind(filter.To.Value, DateTimeKind.Utc);
            query = query.Where(l => l.CreatedAt <= to);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((filter.Page - 1) * filter.Limit)
            .Take(filter.Limit)
            .Select(l => new AdminAuditLogDto
            {
                Id = l.Id,
                UserId = l.user_id,
                Action = l.action,
                EntityType = l.entity_type,
                EntityId = l.entity_id,
                OldValue = l.old_value,
                NewValue = l.new_value,
                IpAddress = l.ip_address,
                CreatedAt = l.CreatedAt,
            })
            .ToListAsync();

        // Join user names in memory (IgnoreQueryFilters so deactivated users' logs still show names)
        var userIds = items.Select(i => i.UserId).Distinct().ToList();
        if (userIds.Count > 0)
        {
            var usersQuery = await _unitOfWork.UsersRepository.GetQueryable();
            var names = await usersQuery
                .IgnoreQueryFilters()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.name })
                .ToDictionaryAsync(x => x.Id, x => x.name);

            foreach (var item in items)
                item.UserName = names.GetValueOrDefault(item.UserId);
        }

        return new PaginationResultDto<AdminAuditLogDto>
        {
            Items = items,
            Page = filter.Page,
            Limit = filter.Limit,
            Total = total,
        };
    }

    public async Task<PaginationResultDto<JobResponseDto>> GetAllJobsAsync(PaginationDto pagination, string? status, string? search)
    {
        var query = await _unitOfWork.JobsRepository.GetQueryable();
        query = query.Include(j => j.entity_Entity);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(j => j.status == status);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(j => j.title.Contains(search) || j.description.Contains(search));

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

        return new PaginationResultDto<JobResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    public async Task DeactivateJobAsync(long id)
    {
        var query = await _unitOfWork.JobsRepository.GetQueryable();
        var exists = await query.AnyAsync(j => j.Id == id);

        if (!exists)
            throw new KeyNotFoundException("الوظيفة غير موجودة");

        // D9: reuse existing "closed" status; D7: batch update bypasses change tracker.
        var now = DateTime.UtcNow;
        await query
            .Where(j => j.Id == id)
            .ExecuteUpdateAsync(j => j
                .SetProperty(x => x.status, "closed")
                .SetProperty(x => x.UpdatedAt, now));
    }
}
