namespace tashrif.Data;

public class application_status_history : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }

    [Required]
    public long application_id { get; set; }

    public string? old_status { get; set; }

    [Required]
    public string new_status { get; set; } = string.Empty;

    [Required]
    public long changed_by { get; set; }

    [Required]
    public DateTime changed_at { get; set; }

    // ISharedColumns
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }

    // Foreign Key
    [ForeignKey("application_id")]
    public applications? application_Entity { get; set; }

    // Foreign Key
    [ForeignKey("changed_by")]
    public users? changed_by_Entity { get; set; }
}
