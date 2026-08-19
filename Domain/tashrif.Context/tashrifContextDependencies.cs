namespace tashrif.Context;

public static class tashrifContextDependencies
{
    public static IServiceCollection AddtashrifContextDependencies(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddDbContext<tashrifDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}
