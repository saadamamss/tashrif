using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs.Auth;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
    [MinLength(8, ErrorMessage = "كلمة المرور الجديدة يجب أن تكون 8 أحرف على الأقل")]
    public string NewPassword { get; set; } = string.Empty;
}
