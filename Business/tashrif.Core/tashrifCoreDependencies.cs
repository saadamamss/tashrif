namespace tashrif.Core;

public static class tashrifCoreDependencies
{
    public static IServiceCollection AddtashrifCoreDependencies(this IServiceCollection services)
    {
        services.AddScoped<IapplicationsService, applicationsService>();
        services.AddScoped<Ibank_accountsService, bank_accountsService>();
        services.AddScoped<Icontact_personsService, contact_personsService>();
        services.AddScoped<IcontractsService, contractsService>();
        services.AddScoped<IcvsService, cvsService>();
        services.AddScoped<Ientity_profilesService, entity_profilesService>();
        services.AddScoped<IexperiencesService, experiencesService>();
        services.AddScoped<Iindividual_profilesService, individual_profilesService>();
        services.AddScoped<IinterviewsService, interviewsService>();
        services.AddScoped<IjobsService, jobsService>();
        services.AddScoped<IqualificationsService, qualificationsService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<INotificationsService, NotificationsService>();
        services.AddScoped<IMessagesService, MessagesService>();
        services.AddScoped<IAdminService, adminService>();
        services.AddScoped<IstatusHistoryService, statusHistoryService>();
        services.AddScoped<IanalyticsService, analyticsService>();
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
