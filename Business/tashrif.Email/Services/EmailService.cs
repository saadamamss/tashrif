using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using tashrif.Email.Interfaces;

namespace tashrif.Email.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            var port = int.Parse(_config["Email:Port"]!);
            var secureSocket = port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;
            await client.ConnectAsync(_config["Email:Smtp"], port, secureSocket);
            await client.AuthenticateAsync(_config["Email:User"], _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
        }
    }

    public async Task<(bool sent, string? error)> TrySendAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            var port = int.Parse(_config["Email:Port"]!);
            var secureSocket = port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;
            await client.ConnectAsync(_config["Email:Smtp"], port, secureSocket);
            await client.AuthenticateAsync(_config["Email:User"], _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return (true, null);
        }
        catch (Exception ex)
        {
            // Unlike SendAsync, the failure is REPORTED to the caller so it can be
            // recorded (D14). The log stays — this is the only signal we have.
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return (false, ex.Message);
        }
    }

    public async Task SendAsync(string[] to, string subject, string htmlBody)
    {
        foreach (var address in to)
        {
            await SendAsync(address, subject, htmlBody);
        }
    }

}