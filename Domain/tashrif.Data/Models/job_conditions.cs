namespace tashrif.Data;

public class job_conditions
 : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string condition_text { get; set; }
    [Required]
    public short sort_order { get; set; }
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


}
