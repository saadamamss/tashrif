using System.ComponentModel.DataAnnotations;

namespace tashrif.Data.DTOs.Auth;

public class RegisterEntityDto
{
    public IFormFile? CompanyLogo { get; set; }
    [Required] public string CompanyName { get; set; } = string.Empty;
    [Required] public string NationalId { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string FieldName { get; set; } = string.Empty;
    [Required] public string Sector { get; set; } = string.Empty;
    [Required] public string Country { get; set; } = string.Empty;
    [Required] public string Province { get; set; } = string.Empty;
    [Required] public string CompanyDesc { get; set; } = string.Empty;
    [Required] public string CompanyWebsite { get; set; } = string.Empty;
    public string? TwitterAccount { get; set; }
    public string? FacebookAccount { get; set; }
    public string? YoutubeAccount { get; set; }
    // Contact person
    [Required] public string Name { get; set; } = string.Empty;
    [Required][EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    [Required] public string Role { get; set; } = string.Empty;
    [Required] public string Nationality { get; set; } = string.Empty;
}
