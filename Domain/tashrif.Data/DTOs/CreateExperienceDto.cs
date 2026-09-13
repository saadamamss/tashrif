using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class CreateExperienceDto
{
    [Required(ErrorMessage = "المسمى الوظيفي مطلوب")]
    [MaxLength(100)]
    public string JobTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "جهة العمل مطلوبة")]
    [MaxLength(200)]
    public string Employer { get; set; } = string.Empty;

    [Required(ErrorMessage = "المدة مطلوبة")]
    [MaxLength(50)]
    public string Duration { get; set; } = string.Empty;

    [Required(ErrorMessage = "الموقع مطلوب")]
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    public bool IsCurrent { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
