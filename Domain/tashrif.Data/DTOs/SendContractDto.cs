namespace tashrif.Data.DTOs;

public class SendContractDto
{
    public long ApplicationId { get; set; }
    public IFormFile? ContractFile { get; set; }
    public string? Notes { get; set; }
    public DateTime? EndDate { get; set; }
}
