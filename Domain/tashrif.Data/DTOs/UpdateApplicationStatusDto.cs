using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class UpdateApplicationStatusDto
{
    [Required(ErrorMessage = "الحالة مطلوبة")]
    [RegularExpression("^(new|shortlisted|interview|contract_sent|accepted|refused|withdrawn)$",
        ErrorMessage = "الحالة غير صحيحة")]
    public string Status { get; set; } = string.Empty;
}
