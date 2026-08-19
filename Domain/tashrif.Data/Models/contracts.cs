namespace tashrif.Data;

public class contracts : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    public string file_url { get; set; }
    public long? file_size { get; set; }
    [Required]
    public string status { get; set; }
    public string? notes { get; set; }
    public DateTime? end_date { get; set; }
    public DateTime? signed_at { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }


    // Foreign Key
    public long job_id { get; set; }
    [ForeignKey("job_id")]
    public jobs job_Entity { get; set; }

    // Foreign Key
    public long application_id { get; set; }
    [ForeignKey("application_id")]
    public applications application_Entity { get; set; }

    // Foreign Key
    public long user_id { get; set; }
    [ForeignKey("user_id")]
    public users user_Entity { get; set; }

    // Foreign Key
    public long entity_id { get; set; }
    [ForeignKey("entity_id")]
    public users entity_Entity { get; set; }



}
