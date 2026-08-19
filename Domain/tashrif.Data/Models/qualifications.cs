namespace tashrif.Data;

public class qualifications : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string type { get; set; }
    public string specialization { get; set; }
    public string institution { get; set; }
    public short? graduation_year { get; set; }
    public string grade { get; set; }
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
