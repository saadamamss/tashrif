namespace tashrif.Repository;

public static class tashrifRepositoryDependencies
{
    public static IServiceCollection AddtashrifRepositoryDependencies(this IServiceCollection services)
    {
        services.AddScoped<IapplicationsRepository, applicationsRepository>();
        services.AddScoped<Ibank_accountsRepository, bank_accountsRepository>();
        services.AddScoped<Icontact_personsRepository, contact_personsRepository>();
        services.AddScoped<IcontractsRepository, contractsRepository>();
        services.AddScoped<IcvsRepository, cvsRepository>();
        services.AddScoped<Ientity_profilesRepository, entity_profilesRepository>();
        services.AddScoped<IexperiencesRepository, experiencesRepository>();
        services.AddScoped<Iindividual_profilesRepository, individual_profilesRepository>();
        services.AddScoped<IinterviewsRepository, interviewsRepository>();
        services.AddScoped<IjobsRepository, jobsRepository>();
        services.AddScoped<Ijob_benefitsRepository, job_benefitsRepository>();
        services.AddScoped<Ijob_conditionsRepository, job_conditionsRepository>();
        services.AddScoped<Ijob_responsibilitiesRepository, job_responsibilitiesRepository>();
        services.AddScoped<IqualificationsRepository, qualificationsRepository>();
        services.AddScoped<IusersRepository, usersRepository>();
        services.AddScoped<Iaudit_logsRepository, audit_logsRepository>();
        services.AddScoped<InotificationsRepository, notificationsRepository>();
        services.AddScoped<ImessagesRepository, messagesRepository>();
        services.AddScoped<Iapplication_status_historyRepository, application_status_historyRepository>();

        services.AddScoped<IGenericRepository<users>, usersRepository>();
        services.AddScoped<IGenericRepository<individual_profiles>, individual_profilesRepository>();
        services.AddScoped<IGenericRepository<entity_profiles>, entity_profilesRepository>();
        services.AddScoped<IGenericRepository<contact_persons>, contact_personsRepository>();
        services.AddScoped<IGenericRepository<audit_logs>, audit_logsRepository>();
        services.AddScoped<IGenericRepository<notifications>, notificationsRepository>();
        services.AddScoped<IGenericRepository<messages>, messagesRepository>();
        services.AddScoped<IGenericRepository<application_status_history>, application_status_historyRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
