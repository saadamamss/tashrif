namespace tashrif.Data;

public class experiences : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string job_title { get; set; }
    [Required]
    public string employer { get; set; }
    public string duration { get; set; }
    public string location { get; set; }
    [Required]
    public bool is_current { get; set; }
    public DateTime? start_date { get; set; }
    public DateTime? end_date { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }

    // Foreign Key
    public long user_id { get; set; }
    [ForeignKey("user_id")]
    public users user_Entity { get; set; }



}
