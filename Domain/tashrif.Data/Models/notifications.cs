namespace tashrif.Data;

public class notifications : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }

    [Required]
    public long user_id { get; set; }

    [Required]
    public string title { get; set; } = string.Empty;

    public string? body { get; set; }

    [Required]
    public string type { get; set; } = string.Empty;

    public long? reference_id { get; set; }

    public string? reference_type { get; set; }

    public bool is_read { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }
}
