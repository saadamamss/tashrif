namespace tashrif.Data;

public class audit_logs : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }

    [Required]
    public long user_id { get; set; }

    [Required]
    public string action { get; set; } = string.Empty;

    [Required]
    public string entity_type { get; set; } = string.Empty;

    [Required]
    public long entity_id { get; set; }

    public string? old_value { get; set; }
    public string? new_value { get; set; }

    public string? ip_address { get; set; }
    public string? user_agent { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }
}
