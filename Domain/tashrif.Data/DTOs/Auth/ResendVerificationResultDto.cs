namespace tashrif.Data.DTOs.Auth;

/// <summary>
/// Service-internal result of a resend attempt (D8/D14).
///
/// <para>
/// The public API always answers with the same generic message, so the caller cannot
/// tell "no such user" from "mail sent". This DTO carries the *real* outcome for the
/// controller: <see cref="ShouldSend"/> says whether a mail must be enqueued, and
/// <see cref="VerificationToken"/> / <see cref="VerificationTokenId"/> carry the freshly
/// minted token so the background task can build the link and record the delivery
/// outcome.
/// </para>
/// </summary>
public class ResendVerificationResultDto
{
    /// <summary>True only when a real, unverified user was found and a token was rotated.</summary>
    public bool ShouldSend { get; set; }

    /// <summary>Raw token of the newly issued token (empty when <see cref="ShouldSend"/> is false).</summary>
    public string VerificationToken { get; set; } = string.Empty;

    /// <summary>Id of the newly issued <c>email_verification_tokens</c> row.</summary>
    public long VerificationTokenId { get; set; }

    /// <summary>Address to mail.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Display name for the mail greeting.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Message describing a previous delivery failure on the *previous* token, if any
    /// (D14 — "surfaced honestly via the resend flow"). Null when there was none.
    /// </summary>
    public string? PreviousDeliveryError { get; set; }
}
