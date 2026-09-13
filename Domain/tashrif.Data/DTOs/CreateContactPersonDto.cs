using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs;

public class CreateContactPersonDto
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الجوال مطلوب")]
    [Phone(ErrorMessage = "رقم الجوال غير صحيح")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "الدور مطلوب")]
    [MaxLength(100)]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "الجنسية مطلوبة")]
    [MaxLength(100)]
    public string Nationality { get; set; } = string.Empty;
}
