namespace tashrif.Email.Interfaces;

public interface IEmailService {
    Task SendAsync (string to, string subject, string htmlBody);
    Task SendAsync (string[] to, string subject, string htmlBody);
}