namespace tashrif.Data.DTOs;

public class EntityProfileResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    // Login id (read-only) — from users.national_id; mirrors IndividualProfileResponseDto.NationalId.
    public string? NationalId { get; set; }
    public string? CompanyField { get; set; }
    public string? CompanySize { get; set; }
    public string? CommercialReg { get; set; }
    public string? Sector { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? District { get; set; }
    public string? Street { get; set; }
    public string? Zipcode { get; set; }
    public string? Region { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public string? LogoUrl { get; set; }
    public short ProfileCompletionPct { get; set; }
    public ContactPersonResponseDto? ContactPerson { get; set; }
    public EntityStatsDto? Stats { get; set; }
}