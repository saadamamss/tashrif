using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs.Auth;

/// <summary>
/// Resend request (D8/D11). Keyed by the national id — the same id the user logs in
/// with — so no session is needed.
/// </summary>
public class ResendVerificationDto
{
    [Required(ErrorMessage = "رقم الهوية مطلوب")]
    public string NationalId { get; set; } = string.Empty;
}
