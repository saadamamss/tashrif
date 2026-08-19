using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs.Auth;

public class RegisterIndividualDto
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required] public string NationalId { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    [Required][EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Gender { get; set; } = string.Empty;
    [Required] public string Nationality { get; set; } = string.Empty;
    public IFormFile? CvFile { get; set; }
    public IFormFile? IdFile { get; set; }
}
