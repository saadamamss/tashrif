using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "رقم الهوية مطلوب")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    public string Password { get; set; } = string.Empty;
}
