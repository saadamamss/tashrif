using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class contractsService(IUnitOfWork unitOfWork, IFileStorageService fileStorage) : IcontractsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorage = fileStorage;

    public async Task<PaginationResultDto<ContractResponseDto>> GetAllAsync(PaginationDto pagination, long userId, string? userType)
    {
        var query = await _unitOfWork.ContractsRepository.GetQueryable();
        query = query.Where(c => !c.IsDeleted)
            .Include(c => c.user_Entity)
            .Include(c => c.job_Entity)
            .Include(c => c.entity_Entity);

        if (userType == "individual")
            query = query.Where(c => c.user_id == userId);
        else if (userType == "entity")
            query = query.Where(c => c.entity_id == userId);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .Select(c => new ContractResponseDto
            {
                Id = c.Id,
                ApplicationId = c.application_id,
                JobId = c.job_id,
                UserId = c.user_id,
                EntityId = c.entity_id,
                FileUrl = c.file_url,
                FileSize = c.file_size,
                UserName = c.user_Entity != null ? c.user_Entity.name : null,
                UserAvatar = c.user_Entity != null ? c.user_Entity.avatar_url : null,
                JobTitle = c.job_Entity != null ? c.job_Entity.title : null,
                EntityName = c.entity_Entity != null ? c.entity_Entity.name : null,
                EntityLogo = c.entity_Entity != null ? c.entity_Entity.avatar_url : null,
                Notes = c.notes,
                EndDate = c.end_date,
                Status = c.status,
                SignedAt = c.signed_at as DateTime?,
                CreatedAt = c.CreatedAt,
            })
            .ToListAsync();

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.FileUrl))
                item.FileName = item.FileUrl.Split('/').LastOrDefault();
        }

        return new PaginationResultDto<ContractResponseDto>
        {
            Items = items,
            Page = pagination.Page,
            Limit = pagination.Limit,
            Total = total,
        };
    }

    public async Task<ContractResponseDto> SendAsync(SendContractDto dto, long entityId)
    {
        var appQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await appQuery.FirstOrDefaultAsync(a => a.Id == dto.ApplicationId && !a.IsDeleted)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        if (app.status == "contract_sent")
            throw new BadHttpRequestException("تم إرسال عقد لهذا الطلب مسبقاً", 400);

        var jobQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var job = await jobQuery.FirstOrDefaultAsync(j => j.Id == app.job_id && !j.IsDeleted)
            ?? throw new KeyNotFoundException("الوظيفة غير موجودة");

        if (job.entity_id != entityId)
            throw new UnauthorizedAccessException("لا تملك صلاحية الوصول");

        string? fileUrl = null;
        if (dto.ContractFile != null)
            fileUrl = await _fileStorage.SaveFileAsync(dto.ContractFile, "contracts");

        var endDate = dto.EndDate.HasValue
            ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        var newContract = new contracts
        {
            application_id = dto.ApplicationId,
            job_id = app.job_id,
            user_id = app.user_id,
            entity_id = entityId,
            file_url = fileUrl ?? "",
            file_size = dto.ContractFile?.Length,
            notes = dto.Notes,
            end_date = endDate,
            status = "sent",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.ContractsRepository.AddAsync(newContract);
        await _unitOfWork.SaveChangesAsync();

        // Use ExecuteUpdateAsync to bypass change tracker — avoids duplicate tracking
        // when AsNoTracking entities are re-queried after SaveChangesAsync auto-tracks them
        var now = DateTime.UtcNow;
        await appQuery.Where(a => a.Id == dto.ApplicationId).ExecuteUpdateAsync(
            a => a.SetProperty(x => x.status, "contract_sent")
                  .SetProperty(x => x.UpdatedAt, now));

        var interviewsQuery = await _unitOfWork.InterviewsRepository.GetQueryable();
        var pendingInterviews = await interviewsQuery
            .Where(i => i.application_id == dto.ApplicationId && !i.IsDeleted && i.status == "scheduled")
            .ToListAsync();

        if (pendingInterviews.Count > 0)
        {
            var pendingIds = pendingInterviews.Select(i => i.Id).ToList();
            var interviewsQ2 = await _unitOfWork.InterviewsRepository.GetQueryable();
            await interviewsQ2.Where(i => pendingIds.Contains(i.Id)).ExecuteUpdateAsync(
                i => i.SetProperty(x => x.status, "completed")
                      .SetProperty(x => x.UpdatedAt, now));
        }

        await _unitOfWork.SaveChangesAsync();

        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == app.user_id);

        var entityQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var entity = await entityQuery.FirstOrDefaultAsync(u => u.Id == entityId);

        return new ContractResponseDto
        {
            Id = newContract.Id,
            ApplicationId = newContract.application_id,
            JobId = newContract.job_id,
            JobTitle = job.title,
            UserId = newContract.user_id,
            UserName = user?.name,
            UserEmail = user?.email,
            EntityId = newContract.entity_id,
            EntityName = entity?.name,
            EntityEmail = entity?.email,
            FileUrl = newContract.file_url,
            FileSize = newContract.file_size,
            Notes = newContract.notes,
            EndDate = endDate,
            Status = newContract.status,
            CreatedAt = newContract.CreatedAt,
        };
    }

    public async Task<ContractResponseDto> SignAsync(long id, long userId)
    {
        var contract = await _unitOfWork.ContractsRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("العقد غير موجود");

        if (contract.user_id != userId)
            throw new UnauthorizedAccessException("غير مصرح لك بتوقيع هذا العقد");

        if (contract.status == "signed")
            throw new BadHttpRequestException("العقد قد تم توقيعه مسبقاً", 400);

        if (contract.end_date.HasValue && DateTime.UtcNow > contract.end_date.Value)
            throw new BadHttpRequestException("انتهت صلاحية توقيع العقد", 400);

        contract.status = "signed";
        contract.signed_at = DateTime.UtcNow;
        contract.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ContractsRepository.Update(contract);

        var now = DateTime.UtcNow;
        var appQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        await appQuery.Where(a => a.Id == contract.application_id && !a.IsDeleted).ExecuteUpdateAsync(
            a => a.SetProperty(x => x.status, "accepted")
                  .SetProperty(x => x.UpdatedAt, now));

        await _unitOfWork.SaveChangesAsync();

        var jobQuery = await _unitOfWork.JobsRepository.GetQueryable();
        var job = await jobQuery.FirstOrDefaultAsync(j => j.Id == contract.job_id);

        var userQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var user = await userQuery.FirstOrDefaultAsync(u => u.Id == contract.user_id);

        var entityQuery = await _unitOfWork.UsersRepository.GetQueryable();
        var entity = await entityQuery.FirstOrDefaultAsync(u => u.Id == contract.entity_id);

        return new ContractResponseDto
        {
            Id = contract.Id,
            ApplicationId = contract.application_id,
            JobId = contract.job_id,
            EntityId = contract.entity_id,
            UserId = contract.user_id,
            JobTitle = job?.title,
            UserName = user?.name,
            UserEmail = user?.email,
            EntityName = entity?.name,
            EntityEmail = entity?.email,
            FileUrl = contract.file_url,
            Status = contract.status,
            SignedAt = contract.signed_at as DateTime?,
            CreatedAt = contract.CreatedAt,
        };
    }
}
