using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface Ibank_accountsService
{
    Task<IEnumerable<BankAccountResponseDto>> GetAllByUserAsync(long userId);
    Task<BankAccountResponseDto> CreateAsync(CreateBankAccountDto dto, long userId);
    Task<BankAccountResponseDto> UpdateAsync(long id, CreateBankAccountDto dto, long userId);
    Task DeleteAsync(long id, long userId);
}
