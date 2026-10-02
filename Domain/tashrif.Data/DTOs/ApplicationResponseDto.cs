namespace tashrif.Data.DTOs;

public class ApplicationResponseDto
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserGender { get; set; } = string.Empty;
    public string UserCity { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public long? CvId { get; set; }
    public string? CvFileName { get; set; }
    public string? CvFilePath { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public JobResponseDto? Job { get; set; }
}
