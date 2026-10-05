using tashrif.Data.DTOs;

namespace tashrif.Core;

public interface IcontractsService
{
    Task<PaginationResultDto<ContractResponseDto>> GetAllAsync(PaginationDto pagination, long userId, string? userType, long? applicationId = null);
    Task<ContractResponseDto> SendAsync(SendContractDto dto, long entityId);
    Task<ContractResponseDto> SignAsync(long id, long userId);
}
