namespace tashrif.Data.DTOs;

public class ContractResponseDto
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public long JobId { get; set; }
    public long UserId { get; set; }
    public long EntityId { get; set; }
    public string? FileUrl { get; set; }
    public long? FileSize { get; set; }
    public string? FileName { get; set; }
    public string? UserName { get; set; }
    public string? UserAvatar { get; set; }
    public string? JobTitle { get; set; }
    public string? EntityName { get; set; }
    public string? EntityLogo { get; set; }
    public string? Notes { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SignedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
