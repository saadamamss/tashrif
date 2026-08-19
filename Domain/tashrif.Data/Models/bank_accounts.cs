namespace tashrif.Data;

public class bank_accounts : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string iban { get; set; }
    [Required]
    public string bank_name { get; set; }
    [Required]
    public string iban_status { get; set; }
    [Required]
    public string account_status { get; set; }
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
