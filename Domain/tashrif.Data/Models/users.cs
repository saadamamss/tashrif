namespace tashrif.Data;

public class users : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string national_id { get; set; }
    [Required]
    public string password_hash { get; set; }
    [Required]
    public string name { get; set; }
    [Required]
    public string email { get; set; }
    public string phone { get; set; }
    [Required]
    public string type { get; set; }
    public string gender { get; set; }
    public string nationality { get; set; }
    public string avatar_url { get; set; }
    public bool must_change_password { get; set; } = true;
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }




}
