using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class ApplyJobDto
{
    [Range(1, long.MaxValue, ErrorMessage = "معرف الوظيفة غير صحيح")]
    public long JobId { get; set; }

    [Required(ErrorMessage = "المؤهل مطلوب")]
    [MaxLength(500)]
    public string Qualification { get; set; } = string.Empty;

    [Required(ErrorMessage = "الخبرة مطلوبة")]
    [MaxLength(500)]
    public string Experience { get; set; } = string.Empty;
}
