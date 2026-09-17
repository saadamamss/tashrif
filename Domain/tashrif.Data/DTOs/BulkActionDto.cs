using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class BulkActionDto
{
    [Required(ErrorMessage = "معرفات الطلبات مطلوبة")]
    [MinLength(1, ErrorMessage = "يجب تحديد طلب واحد على الأقل")]
    public List<long> ApplicationIds { get; set; } = new();

    [Required(ErrorMessage = "الإجراء مطلوب")]
    [RegularExpression("^(shortlist|refuse|restore)$",
        ErrorMessage = "الإجراء غير صحيح — مسموح فقط: shortlist, refuse, restore")]
    public string Action { get; set; } = string.Empty;
}
