namespace SurveyBasket.API.Persistences.Context;

public class SurveyBasketDbContext(DbContextOptions<SurveyBasketDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Poll> Polls {  get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
