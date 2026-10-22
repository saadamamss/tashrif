namespace tashrif.Data;

public class email_verification_tokens : ISharedColumns
{
    [Key]
    [Required]
    public long Id { get; set; }

    [Required]
    public long user_id { get; set; }

    /// <summary>
    /// SHA-256 hex digest of the raw verification token. The raw token is never
    /// persisted — it exists only in the email link (D4).
    /// </summary>
    [Required]
    public string token_hash { get; set; }

    /// <summary>Expiry of the raw token (issued_at + 24h).</summary>
    [Required]
    public DateTime expires_at { get; set; }

    /// <summary>Set when the token is redeemed. NULL while the token is still live.</summary>
    public DateTime? used_at { get; set; }

    /// <summary>Set when the verification mail was handed to the mail queue (D14).</summary>
    public DateTime? sent_at { get; set; }

    /// <summary>Delivery failure detail (D14). NULL while delivery is healthy.</summary>
    public string? last_error { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedTime { get; set; }

    // Foreign Key
    [ForeignKey("user_id")]
    public users? user_Entity { get; set; }
}
