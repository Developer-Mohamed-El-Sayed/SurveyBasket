namespace SurveyBasket.API.Persistences.EntitiesConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder
            .HasKey(pk => pk.Id);
        builder
            .Property(pk => pk.Id)
            .UseIdentityColumn();
        builder
            .HasIndex(x => new { x.PollId, x.UserId })
            .IsUnique();
    }
}
