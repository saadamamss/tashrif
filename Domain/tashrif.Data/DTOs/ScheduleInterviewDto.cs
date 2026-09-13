using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class ScheduleInterviewDto
{
    [Range(1, long.MaxValue, ErrorMessage = "معرف الطلب غير صحيح")]
    public long ApplicationId { get; set; }

    [Required(ErrorMessage = "طريقة المقابلة مطلوبة")]
    [MaxLength(20)]
    public string Method { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاريخ المقابلة مطلوب")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "وقت المقابلة مطلوب")]
    [MaxLength(20)]
    public string Time { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(500)]
    public string? Link { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
