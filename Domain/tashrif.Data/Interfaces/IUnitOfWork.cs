namespace tashrif.Data.Interfaces;
public interface IUnitOfWork
{
   IapplicationsRepository ApplicationsRepository { get; }
   Ibank_accountsRepository Bank_accountsRepository { get; }
   Icontact_personsRepository Contact_personsRepository { get; }
   IcontractsRepository ContractsRepository { get; }
   IcvsRepository CvsRepository { get; }
   Ientity_profilesRepository Entity_profilesRepository { get; }
   IexperiencesRepository ExperiencesRepository { get; }
   Iindividual_profilesRepository Individual_profilesRepository { get; }
   IinterviewsRepository InterviewsRepository { get; }
    IjobsRepository JobsRepository { get; }
    Ijob_benefitsRepository Job_benefitsRepository { get; }
    Ijob_conditionsRepository Job_conditionsRepository { get; }
    Ijob_responsibilitiesRepository Job_responsibilitiesRepository { get; }
    IqualificationsRepository QualificationsRepository { get; }
    IusersRepository UsersRepository { get; }
    Task<int> SaveChangesAsync();
}
