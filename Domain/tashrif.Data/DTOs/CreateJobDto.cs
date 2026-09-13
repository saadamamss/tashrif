using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class CreateJobDto
{
    [Required(ErrorMessage = "عنوان الوظيفة مطلوب")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "وصف الوظيفة مطلوب")]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "الموقع مطلوب")]
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع العمل مطلوب")]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "الراتب مطلوب")]
    [MaxLength(100)]
    public string Salary { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "عدد الوظائف يجب أن يكون بين 1 و 10000")]
    public int Vacancies { get; set; }

    [Required(ErrorMessage = "الهدف مطلوب")]
    [MaxLength(100)]
    public string Target { get; set; } = string.Empty;

    [Required(ErrorMessage = "المؤهل المطلوب مطلوب")]
    [MaxLength(200)]
    public string Qualification { get; set; } = string.Empty;

    [Required(ErrorMessage = "الجنس مطلوب")]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "ساعات العمل مطلوبة")]
    [MaxLength(50)]
    public string Hours { get; set; } = string.Empty;

    [Required(ErrorMessage = "المدة مطلوبة")]
    [MaxLength(50)]
    public string Duration { get; set; } = string.Empty;

    public DateTime? EndDate { get; set; }
    public List<string> Benefits { get; set; } = new();
    public List<string> Responsibilities { get; set; } = new();
    public List<string> Conditions { get; set; } = new();
}
