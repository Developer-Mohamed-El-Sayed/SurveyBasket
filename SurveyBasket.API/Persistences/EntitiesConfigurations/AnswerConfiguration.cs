namespace SurveyBasket.API.Persistences.EntitiesConfigurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
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
            .HasIndex(i => new { i.QuestionId, i.Content })
            .IsUnique();

    }
}
