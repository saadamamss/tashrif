using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs.Auth;

public class RegisterEntityDto
{
    public IFormFile? CompanyLogo { get; set; }

    [Required(ErrorMessage = "اسم الشركة مطلوب")]
    [MaxLength(100)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الهوية مطلوب")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "رقم الهوية يجب أن يكون 10 أرقام")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "مجال العمل مطلوب")]
    [MaxLength(100)]
    public string FieldName { get; set; } = string.Empty;

    [Required(ErrorMessage = "القطاع مطلوب")]
    [MaxLength(100)]
    public string Sector { get; set; } = string.Empty;

    [Required(ErrorMessage = "الدولة مطلوبة")]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "المنطقة مطلوبة")]
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;

    [Required(ErrorMessage = "وصف الشركة مطلوب")]
    [MaxLength(2000)]
    public string CompanyDesc { get; set; } = string.Empty;

    [Required(ErrorMessage = "موقع الشركة مطلوب")]
    [Url(ErrorMessage = "رابط الموقع غير صحيح")]
    public string CompanyWebsite { get; set; } = string.Empty;

    [Url(ErrorMessage = "رابط تويتر غير صحيح")]
    public string? TwitterAccount { get; set; }

    [Url(ErrorMessage = "رابط فيسبوك غير صحيح")]
    public string? FacebookAccount { get; set; }

    [Url(ErrorMessage = "رابط يوتيوب غير صحيح")]
    public string? YoutubeAccount { get; set; }

    // Contact person
    [Required(ErrorMessage = "اسم جهة الاتصال مطلوب")]
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
