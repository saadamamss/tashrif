using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class CreateBankAccountDto
{
    [Required(ErrorMessage = "رقم الآيبان مطلوب")]
    [MaxLength(34)]
    public string Iban { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم البنك مطلوب")]
    [MaxLength(100)]
    public string BankName { get; set; } = string.Empty;
}
