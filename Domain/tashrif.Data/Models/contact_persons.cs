namespace tashrif.Data;

public class contact_persons : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string name { get; set; }
    public string role { get; set; }
    public string nationality { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    [Required]
    public bool is_primary { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }

    // Foreign Key
    public long entity_id { get; set; }
    [ForeignKey("entity_id")]
    public entity_profiles entity_Entity { get; set; }



}
