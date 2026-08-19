namespace tashrif.Data.DTOs;

public class InterviewResponseDto
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public long JobId { get; set; }
    public long UserId { get; set; }
    public long EntityId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityLogo { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Link { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Attendance { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
