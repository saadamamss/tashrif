namespace tashrif.Data;

public class cvs : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string file_name { get; set; }
    [Required]
    public string file_path { get; set; }
    public long? file_size { get; set; }
    [Required]
    public bool is_default { get; set; }
    [Required]
    public DateTime UploadedAt { get; set; }
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
