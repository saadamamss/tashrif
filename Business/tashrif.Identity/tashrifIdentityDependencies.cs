using tashrif.Identity.Interfaces;
using tashrif.Identity.Services;

namespace tashrif.Identity;

public static class tashrifIdentityDependencies
{
    public static IServiceCollection AddtashrifIdentityDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
