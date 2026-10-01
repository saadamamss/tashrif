using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs.Auth;

public class RegisterIndividualDto
{
    [Required(ErrorMessage = "الاسم الأول مطلوب")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم العائلة مطلوب")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الهوية مطلوب")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "رقم الهوية يجب أن يكون 10 أرقام")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الجوال مطلوب")]
    [Phone(ErrorMessage = "رقم الجوال غير صحيح")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "الجنس مطلوب")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "الجنسية مطلوبة")]
    public string Nationality { get; set; } = string.Empty;

    public IFormFile? IdFile { get; set; }
}
