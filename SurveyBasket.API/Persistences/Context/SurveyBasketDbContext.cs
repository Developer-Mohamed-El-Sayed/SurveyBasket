namespace SurveyBasket.API.Persistences.Context;

public class SurveyBasketDbContext(DbContextOptions<SurveyBasketDbContext> options,IHttpContextAccessor httpContextAccessor) 
    : IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Poll> Polls {  get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Auditable>();

        foreach (var entry in entries)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()!;
            if (entry.State == EntityState.Added)
            {
                entry.Property(c => c.CreatedById).CurrentValue = currentUserId;
            }
            else if(entry.State == EntityState.Modified)
            {
                entry.Property(u => u.UpdatedById).CurrentValue = currentUserId;
                entry.Property(u => u.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
