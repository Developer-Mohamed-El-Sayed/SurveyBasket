namespace SurveyBasket.API.Persistences.EntitiesConfigurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder
            .HasKey(pk => pk.Id);
        builder
            .Property(pk => pk.Id)
            .UseIdentityColumn();
        builder
            .Property(c => c.Content)
            .HasMaxLength(1000);
        builder
            .HasIndex(i => new { i.PollId, i.Content })
            .IsUnique();

    }
}
