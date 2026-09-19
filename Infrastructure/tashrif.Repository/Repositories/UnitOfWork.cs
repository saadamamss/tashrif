namespace tashrif.Repository;
public class UnitOfWork : IUnitOfWork
{
    #region Private Fields & Properties

    private readonly tashrifDBContext _context;
    public IapplicationsRepository ApplicationsRepository { get; }
    public Ibank_accountsRepository Bank_accountsRepository { get; }
    public Icontact_personsRepository Contact_personsRepository { get; }
    public IcontractsRepository ContractsRepository { get; }
    public IcvsRepository CvsRepository { get; }
    public Ientity_profilesRepository Entity_profilesRepository { get; }
    public IexperiencesRepository ExperiencesRepository { get; }
    public Iindividual_profilesRepository Individual_profilesRepository { get; }
    public IinterviewsRepository InterviewsRepository { get; }
    public IjobsRepository JobsRepository { get; }
    public Ijob_benefitsRepository Job_benefitsRepository { get; }
    public Ijob_conditionsRepository Job_conditionsRepository { get; }
    public Ijob_responsibilitiesRepository Job_responsibilitiesRepository { get; }
    public IqualificationsRepository QualificationsRepository { get; }
    public IusersRepository UsersRepository { get; }
    public Iaudit_logsRepository Audit_logsRepository { get; }
    public InotificationsRepository NotificationsRepository { get; }
    public ImessagesRepository MessagesRepository { get; }
    public Iapplication_status_historyRepository Application_status_historyRepository { get; }

    #endregion

    #region Constructor

    public UnitOfWork(tashrifDBContext context,
        IapplicationsRepository applicationsRepository,
        Ibank_accountsRepository bank_accountsRepository,
        Icontact_personsRepository contact_personsRepository,
        IcontractsRepository contractsRepository,
        IcvsRepository cvsRepository,
        Ientity_profilesRepository entity_profilesRepository,
        IexperiencesRepository experiencesRepository,
        Iindividual_profilesRepository individual_profilesRepository,
        IinterviewsRepository interviewsRepository,
        IjobsRepository jobsRepository,
        Ijob_benefitsRepository job_benefitsRepository,
        Ijob_conditionsRepository job_conditionsRepository,
        Ijob_responsibilitiesRepository job_responsibilitiesRepository,
        IqualificationsRepository qualificationsRepository,
        IusersRepository usersRepository,
        Iaudit_logsRepository audit_logsRepository,
        InotificationsRepository notificationsRepository,
        ImessagesRepository messagesRepository,
        Iapplication_status_historyRepository application_status_historyRepository
    )
    {
        _context = context;
        ApplicationsRepository = applicationsRepository;
        Bank_accountsRepository = bank_accountsRepository;
        Contact_personsRepository = contact_personsRepository;
        ContractsRepository = contractsRepository;
        CvsRepository = cvsRepository;
        Entity_profilesRepository = entity_profilesRepository;
        ExperiencesRepository = experiencesRepository;
        Individual_profilesRepository = individual_profilesRepository;
        InterviewsRepository = interviewsRepository;
        JobsRepository = jobsRepository;
        Job_benefitsRepository = job_benefitsRepository;
        Job_conditionsRepository = job_conditionsRepository;
        Job_responsibilitiesRepository = job_responsibilitiesRepository;
        QualificationsRepository = qualificationsRepository;
        UsersRepository = usersRepository;
        Audit_logsRepository = audit_logsRepository;
        NotificationsRepository = notificationsRepository;
        MessagesRepository = messagesRepository;
        Application_status_historyRepository = application_status_historyRepository;
    }

    #endregion

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
