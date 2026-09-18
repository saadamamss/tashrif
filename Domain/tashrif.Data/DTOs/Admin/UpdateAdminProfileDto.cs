using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs.Admin;

public class UpdateAdminProfileDto
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    [MaxLength(200, ErrorMessage = "الحد الأقصى للاسم 200 حرف")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
    [MaxLength(200, ErrorMessage = "الحد الأقصى للبريد الإلكتروني 200 حرف")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "رقم الجوال غير صالح")]
    [MaxLength(20, ErrorMessage = "الحد الأقصى لرقم الجوال 20 رقم")]
    public string? Phone { get; set; }

    public string? Gender { get; set; }

    [MaxLength(100, ErrorMessage = "الحد الأقصى للجنسية 100 حرف")]
    public string? Nationality { get; set; }
}
