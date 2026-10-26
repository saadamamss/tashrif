namespace tashrif.Email.Interfaces;

public interface IEmailService {
    Task SendAsync (string to, string subject, string htmlBody);
    Task SendAsync (string[] to, string subject, string htmlBody);

    /// <summary>
    /// Sends a mail and reports whether it actually went out.
    ///
    /// <para>
    /// <see cref="SendAsync"/> swallows every exception (it logs and returns), so a
    /// caller cannot tell a delivered mail from a failed one. The email-verification
    /// flow needs that signal (D14): a failure must be recorded on the token row
    /// (<c>sent_at</c> / <c>last_error</c>) instead of silently locking the user out.
    /// </para>
    ///
    /// <para>
    /// This is additive on purpose — <see cref="SendAsync"/> and its existing callers
    /// (Contracts, Interviews, Applications) are left untouched.
    /// </para>
    /// </summary>
    /// <returns>
    /// A tuple: <c>sent</c> is <c>true</c> when the message was handed to the SMTP server;
    /// <c>error</c> carries the failure reason (null on success).
    /// </returns>
    Task<(bool sent, string? error)> TrySendAsync(string to, string subject, string htmlBody);
}