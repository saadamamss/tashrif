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
    /// <summary>
    /// True once the email on this row has been proven (D2). New registrations start
    /// FALSE and are flipped by the verify-email flow; pre-existing users were
    /// backfilled TRUE by the AddEmailVerification migration (D9).
    /// </summary>
    public bool is_email_verified { get; set; } = false;
    public string phone { get; set; }
    [Required]
    public string type { get; set; }
    public string gender { get; set; }
    public string nationality { get; set; }
    public string avatar_url { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }




}
