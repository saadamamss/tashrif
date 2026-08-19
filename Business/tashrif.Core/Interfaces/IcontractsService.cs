using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IcontractsService
{
    Task<PaginationResultDto<ContractResponseDto>> GetAllAsync(PaginationDto pagination, long userId, string? userType);
    Task<ContractResponseDto> SendAsync(SendContractDto dto, long entityId);
    Task<ContractResponseDto> SignAsync(long id, long userId);
}
