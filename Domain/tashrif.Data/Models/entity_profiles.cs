namespace tashrif.Data;

public class entity_profiles : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }
    public string company_field { get; set; }
    public string sector { get; set; }
    public string company_size { get; set; }
    public string commercial_reg { get; set; }
    public string country { get; set; }
    public string city { get; set; }
    public string zone { get; set; }
    public string district { get; set; }
    public string street { get; set; }
    public string zipcode { get; set; }
    public string website { get; set; }
    public string facebook_url { get; set; }
    public string twitter_url { get; set; }
    public string youtube_url { get; set; }
    public string logo_url { get; set; }
    public string province { get; set; }
    public string company_desc { get; set; }
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
