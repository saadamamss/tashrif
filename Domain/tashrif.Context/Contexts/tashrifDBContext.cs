namespace tashrif.Context;
public class tashrifDBContext : DbContext
{

   #region Fields & Properties
   
   #endregion

   #region Constructors

   public tashrifDBContext(DbContextOptions<tashrifDBContext> options) : base(options)
   {
   
   }
   #endregion

   #region Entities(Models)

   public DbSet<applications> applications => Set<applications>();
   public DbSet<bank_accounts> bank_accounts => Set<bank_accounts>();
   public DbSet<contact_persons> contact_persons => Set<contact_persons>();
   public DbSet<contracts> contracts => Set<contracts>();
   public DbSet<cvs> cvs => Set<cvs>();
   public DbSet<entity_profiles> entity_profiles => Set<entity_profiles>();
   public DbSet<experiences> experiences => Set<experiences>();
   public DbSet<individual_profiles> individual_profiles => Set<individual_profiles>();
   public DbSet<interviews> interviews => Set<interviews>();
   public DbSet<jobs> jobs => Set<jobs>();
   public DbSet<job_benefits> job_benefits => Set<job_benefits>();
   public DbSet<job_conditions> job_conditions => Set<job_conditions>();
   public DbSet<job_responsibilities> job_responsibilities => Set<job_responsibilities>();
   public DbSet<qualifications> qualifications => Set<qualifications>();
   public DbSet<users> users => Set<users>();
   public DbSet<audit_logs> audit_logs => Set<audit_logs>();
   public DbSet<notifications> notifications => Set<notifications>();
   public DbSet<messages> messages => Set<messages>();

   #endregion

   #region OnModelCreating

   protected override void OnModelCreating(ModelBuilder builder)
   {
       #region Global Filter

       foreach (var entityType in builder.Model.GetEntityTypes())
       {
           if (typeof(ISharedColumns).IsAssignableFrom(entityType.ClrType) && entityType.BaseType == null)
           {
               builder.Entity(entityType.ClrType).HasQueryFilter(CreateIsDeletedFilter(entityType.ClrType));
           }
       }

       #endregion

       base.OnModelCreating(builder);
   }

   #endregion

   private static LambdaExpression CreateIsDeletedFilter(Type entityType)
   {
       var parameter = Expression.Parameter(entityType, "e");
       var property = Expression.Property(parameter, nameof(ISharedColumns.IsDeleted));
       var comparison = Expression.MakeBinary(ExpressionType.Equal, property, Expression.Constant(false));
       return Expression.Lambda(comparison, parameter);
   }

}
