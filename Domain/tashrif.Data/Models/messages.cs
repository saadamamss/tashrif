namespace tashrif.Data;

public class messages : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }

    [Required]
    public long application_id { get; set; }

    [Required]
    public long sender_id { get; set; }

    [Required]
    public string body { get; set; } = string.Empty;

    public bool is_read { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }

    [ForeignKey("application_id")]
    public applications? application_entity { get; set; }

    [ForeignKey("sender_id")]
    public users? sender_entity { get; set; }
}
