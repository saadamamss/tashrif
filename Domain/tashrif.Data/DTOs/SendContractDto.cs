using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class SendContractDto
{
    [Range(1, long.MaxValue, ErrorMessage = "معرف الطلب غير صحيح")]
    public long ApplicationId { get; set; }

    public IFormFile? ContractFile { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime? EndDate { get; set; }
}
