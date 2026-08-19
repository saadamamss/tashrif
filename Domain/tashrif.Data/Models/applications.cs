namespace tashrif.Data;

public class applications : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    public string qualification { get; set; }
    public string experience { get; set; }
    public string cover_letter { get; set; }
    [Required]
    public string status { get; set; }
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
    public long user_id { get; set; }
    [ForeignKey("user_id")]
    public users user_Entity { get; set; }

    // Foreign Key
    public long? cv_id { get; set; }
    [ForeignKey("cv_id")]
    public cvs? cv_Entity { get; set; }



}
