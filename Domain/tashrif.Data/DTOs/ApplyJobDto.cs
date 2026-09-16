using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class ApplyJobDto
{
    [Range(1, long.MaxValue, ErrorMessage = "معرف الوظيفة غير صحيح")]
    public long JobId { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "يجب اختيار المؤهل")]
    public long QualificationId { get; set; }

    [Required(ErrorMessage = "الخبرة مطلوبة")]
    [MaxLength(500)]
    public string Experience { get; set; } = string.Empty;
}
