namespace SurveyBasket.API.Persistences.EntitiesConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .Property(f => f.FirstName)
            .HasMaxLength(100);
        builder
            .Property(l => l.LastName)
            .HasMaxLength(100);
    }
}
