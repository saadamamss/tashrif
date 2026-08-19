namespace tashrif.Data;

public class jobs : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string title { get; set; }
    [Required]
    public string description { get; set; }
    [Required]
    public string location { get; set; }
    [Required]
    public string work_type { get; set; }
    public string target { get; set; }
    [Required]
    public int vacancies { get; set; }
    public string qualification { get; set; }
    public decimal? salary_min { get; set; }
    public decimal? salary_max { get; set; }
    public string salary_text { get; set; }
    public string gender { get; set; }
    public string hours { get; set; }
    public string duration { get; set; }
    [Required]
    public string status { get; set; }
    public DateTime? publish_date { get; set; }
    public DateTime? end_date { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }


    // Foreign Key
    public long entity_id { get; set; }
    [ForeignKey("entity_id")]
    public users entity_Entity { get; set; }



}
