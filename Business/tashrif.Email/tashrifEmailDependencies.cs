using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tashrif.Email.Interfaces;
using tashrif.Email.Services;

namespace tashrif.Email;

public static class tashrifEmailDependencies
{
    public static IServiceCollection AddtashrifEmailDependencies (this IServiceCollection services)
    {
        services.AddHttpClient();

        // Email:Provider picks the transport:
        //   "resend" → HTTPS API     (hosts without SMTP egress, e.g. Railway Free/Hobby)
        //   default  → SMTP SendAsync (local dev user-secrets, unchanged)
        //
        services.AddScoped<IEmailService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var provider = config["Email:Provider"] ?? "smtp";
            return provider.Equals("resend", StringComparison.OrdinalIgnoreCase)
                ? new ResendEmailService(
                    config,
                    sp.GetRequiredService<ILogger<ResendEmailService>>(),
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("resend"))
                : new EmailService(
                    config,
                    sp.GetRequiredService<ILogger<EmailService>>());
        });
        return services;
    }
}