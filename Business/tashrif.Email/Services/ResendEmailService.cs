using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using tashrif.Email.Interfaces;

namespace tashrif.Email.Services;

/// <summary>
/// IEmailService backed by Resend's HTTPS HTTPS API instead of SMTP.
///
/// <para>
/// Reason this exists: Railway disables outbound SMTP (ports 25/465/587) on every plan
/// below Pro — connections never leave the platform and always time out. HTTPS egress is
/// allowed on all plans, and Resend lives at https://api.resend.com/emails.
/// </para>
///
/// <para>
/// Selected in DI when <c>Email:Provider == "resend"</c> (Access/Trial/Hobby) —
/// <see cref="EmailService"/> (SMTP) remains the default for local dev.
///
/// Required prod config (Railway env vars):
///   Email__Provider=resend
///   Email__ApiKey=re_...      (Resend API key, dashboard → API keys)
///   Email__From=...           — MUST be an address on a domain verified in Resend;
///                               with only a gmail.com sender, Resend only delivers
///                               to the account owner's own address.
/// </para>
/// </summary>
public class ResendEmailService : IEmailService
{
    private readonly HttpClient _http;
    private readonly string _from;
    private readonly string _apiKey;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(IConfiguration config, ILogger<ResendEmailService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _http = httpClient;

        // Fail fast on config, not at send time. A missing key would otherwise swallow
        // every email into the send-time catch and users would "never get verified".
        _from = config["Email:From"] is { Length: > 0 } from
            ? from
            : throw new InvalidOperationException("Missing required configuration: Email:From");
        _apiKey = config["Email:ApiKey"] is { Length: > 0 } key
            ? key
            : throw new InvalidOperationException(
                "Missing required configuration: Email:ApiKey (set Email__ApiKey on the host when Email__Provider=resend)");
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        // Mirrors EmailService.SendAsync semantics: errors are logged, never thrown —
        // callers (Contracts/Applications/Interviews fire-and-forget) rely on that.
        await SendCoreAsync([to], subject, htmlBody);
    }

    /// <summary>One HTTPS request for the whole batch (Resend accepts a recipient array) —
    /// stays inside the API rate limit where the SMTP version looped per-address.</summary>
    public async Task SendAsync(string[] to, string subject, string htmlBody)
    {
        await SendCoreAsync(to, subject, htmlBody);
    }

    public async Task<(bool sent, string? error)> TrySendAsync(string to, string subject, string htmlBody)
    {
        return await SendCoreAsync([to], subject, htmlBody);
    }

    private async Task<(bool sent, string? error)> SendCoreAsync(IReadOnlyList<string> recipients, string subject, string htmlBody)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            // JsonContent uses web defaults → camelCase property names as Resend expects.
            request.Content = JsonContent.Create(new
            {
                from = _from,
                to = recipients,
                subject,
                html = htmlBody,
            });

            using var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var body = await response.Content.ReadAsStringAsync();
            _logger.LogError("Resend rejected mail to {To}: HTTP {Status} {Body}",
                string.Join(", ", recipients), (int)response.StatusCode, body);
            return (false, $"Resend HTTP {(int)response.StatusCode}: {body}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resend send failed for {To}", string.Join(", ", recipients));
            return (false, ex.Message);
        }
    }
}
