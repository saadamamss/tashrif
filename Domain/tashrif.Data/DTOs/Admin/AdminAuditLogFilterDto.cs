namespace tashrif.Data.DTOs.Admin;

public class AdminAuditLogFilterDto : PaginationDto
{
    public long? UserId { get; set; }
    public string? Action { get; set; }
    public string? EntityType { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
