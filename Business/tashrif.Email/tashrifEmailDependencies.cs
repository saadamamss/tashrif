using Microsoft.Extensions.DependencyInjection;
using tashrif.Email.Interfaces;
using tashrif.Email.Services;

namespace tashrif.Email;

public static class tashrifEmailDependencies
{
    public static IServiceCollection AddtashrifEmailDependencies (this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}