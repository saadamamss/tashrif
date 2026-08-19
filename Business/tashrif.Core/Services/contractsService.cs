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

        app.status = "contract_sent";
        _unitOfWork.ApplicationsRepository.Update(app);

        var interviewsQuery = await _unitOfWork.InterviewsRepository.GetQueryable();
        var pendingInterviews = await interviewsQuery
            .Where(i => i.application_id == dto.ApplicationId && !i.IsDeleted && i.status == "scheduled")
            .ToListAsync();
        foreach (var interview in pendingInterviews)
        {
            interview.status = "completed";
            interview.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.InterviewsRepository.Update(interview);
        }

        await _unitOfWork.SaveChangesAsync();

        return new ContractResponseDto
        {
            Id = newContract.Id,
            ApplicationId = newContract.application_id,
            JobId = newContract.job_id,
            UserId = newContract.user_id,
            EntityId = newContract.entity_id,
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

        if (contract.end_date.HasValue && DateTime.UtcNow > contract.end_date.Value)
            throw new BadHttpRequestException("انتهت صلاحية توقيع العقد", 400);

        contract.status = "signed";
        contract.signed_at = DateTime.UtcNow;
        contract.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ContractsRepository.Update(contract);

        var appQuery = await _unitOfWork.ApplicationsRepository.GetQueryable();
        var app = await appQuery.FirstOrDefaultAsync(a => a.Id == contract.application_id && !a.IsDeleted);
        if (app != null)
        {
            app.status = "accepted";
            app.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.ApplicationsRepository.Update(app);
        }

        await _unitOfWork.SaveChangesAsync();

        return new ContractResponseDto
        {
            Id = contract.Id,
            ApplicationId = contract.application_id,
            JobId = contract.job_id,
            UserId = contract.user_id,
            EntityId = contract.entity_id,
            FileUrl = contract.file_url,
            Status = contract.status,
            SignedAt = contract.signed_at as DateTime?,
            CreatedAt = contract.CreatedAt,
        };
    }
}
