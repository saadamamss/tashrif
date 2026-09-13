using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class CreateQualificationDto
{
    [Required(ErrorMessage = "نوع المؤهل مطلوب")]
    [MaxLength(100)]
    public string QualificationType { get; set; } = string.Empty;

    [Required(ErrorMessage = "التخصص مطلوب")]
    [MaxLength(200)]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "المؤسسة التعليمية مطلوبة")]
    [MaxLength(200)]
    public string Institution { get; set; } = string.Empty;

    [Required(ErrorMessage = "التقدير مطلوب")]
    [MaxLength(50)]
    public string Grade { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "سنة التخرج غير صحيحة")]
    public short? GraduationYear { get; set; }
}
