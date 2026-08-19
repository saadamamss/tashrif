namespace tashrif.Data;

public class individual_profiles : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    public DateTime? birth_date { get; set; }
    public string city { get; set; }
    public string zone { get; set; }
    public string district { get; set; }
    public string street { get; set; }
    public string zipcode { get; set; }
    public string job_title { get; set; }
    public string cv_file { get; set; }
    public string id_file { get; set; }
    [Required]
    public short profile_completion_pct { get; set; }
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
