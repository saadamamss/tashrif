using Microsoft.EntityFrameworkCore;

namespace tashrif.Core;

public class bank_accountsService(IUnitOfWork unitOfWork) : Ibank_accountsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<BankAccountResponseDto>> GetAllByUserAsync(long userId)
    {
        var query = await _unitOfWork.Bank_accountsRepository.GetQueryable();
        return await query
            .Where(b => b.user_id == userId && !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BankAccountResponseDto
            {
                Id = b.Id,
                UserId = b.user_id,
                Iban = b.iban,
                BankName = b.bank_name,
                IbanStatus = b.iban_status,
                AccountStatus = b.account_status,
                CreatedAt = b.CreatedAt,
            })
            .ToListAsync();
    }

    public async Task<BankAccountResponseDto> CreateAsync(CreateBankAccountDto dto, long userId)
    {
        var entity = new bank_accounts
        {
            user_id = userId,
            iban = dto.Iban,
            bank_name = dto.BankName,
            iban_status = "pending",
            account_status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.Bank_accountsRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<BankAccountResponseDto> UpdateAsync(long id, CreateBankAccountDto dto, long userId)
    {
        var query = await _unitOfWork.Bank_accountsRepository.GetQueryable();
        var entity = await query
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted)
            ?? throw new KeyNotFoundException("الحساب البنكي غير موجود");

        if (entity.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية تعديل هذا الحساب البنكي");

        entity.iban = dto.Iban;
        entity.bank_name = dto.BankName;
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Bank_accountsRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(long id, long userId)
    {
        var query = await _unitOfWork.Bank_accountsRepository.GetQueryable();
        var entity = await query
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted)
            ?? throw new KeyNotFoundException("الحساب البنكي غير موجود");

        if (entity.user_id != userId)
            throw new UnauthorizedAccessException("لا تملك صلاحية حذف هذا الحساب البنكي");

        entity.IsDeleted = true;
        entity.DeletedTime = DateTime.UtcNow;
        _unitOfWork.Bank_accountsRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    private static BankAccountResponseDto Map(bank_accounts b)
    {
        return new BankAccountResponseDto
        {
            Id = b.Id,
            UserId = b.user_id,
            Iban = b.iban,
            BankName = b.bank_name,
            IbanStatus = b.iban_status,
            AccountStatus = b.account_status,
            CreatedAt = b.CreatedAt,
        };
    }
}