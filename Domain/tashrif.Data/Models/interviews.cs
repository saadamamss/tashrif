namespace tashrif.Data;

public class interviews : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string method { get; set; }
    [Required]
    public DateTime interview_date { get; set; }
    [Required]
    public string interview_time { get; set; }
    public string location { get; set; }
    public string link { get; set; }
    public string notes { get; set; }
    [Required]
    public string status { get; set; }
    [Required]
    public string attendance { get; set; }
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
