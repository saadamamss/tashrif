using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class UpdateContractDto
{
    public IFormFile? ContractFile { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime? EndDate { get; set; }
}
