namespace tashrif.Data.DTOs.Auth;

/// <summary>
/// Result of a successful registration (D13).
///
/// <para>
/// The <see cref="VerificationToken"/> is the RAW token — it exists only in the email
/// link and is never persisted in plaintext. It travels service → controller in memory
/// and is stripped before the HTTP response is serialized (the body carries only
/// <see cref="Message"/>).
/// </para>
/// </summary>
public class RegisterResponseDto
{
    /// <summary>Arabic message shown to the user after registering.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Raw 256-bit token — put it in the verification link, nowhere else.</summary>
    public string VerificationToken { get; set; } = string.Empty;

    /// <summary>
    /// Id of the <c>email_verification_tokens</c> row that backs <see cref="VerificationToken"/>.
    /// Used by the mail task to record the delivery outcome (D14: sent_at / last_error).
    /// </summary>
    public long VerificationTokenId { get; set; }

    /// <summary>Address the verification mail is sent to.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Display name used in the mail greeting.</summary>
    public string Name { get; set; } = string.Empty;
}
