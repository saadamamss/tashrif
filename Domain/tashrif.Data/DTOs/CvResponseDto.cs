namespace tashrif.Data.DTOs;

public class CvResponseDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public bool IsDefault { get; set; }
    public DateTime UploadedAt { get; set; }
}
