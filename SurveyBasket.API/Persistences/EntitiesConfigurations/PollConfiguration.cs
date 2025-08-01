﻿namespace SurveyBasket.API.Persistences.EntitiesConfigurations;

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder
            .HasIndex(t => t.Title);
        builder.
            HasKey(pk => pk.Id);
        builder.Property(pk => pk.Id)
            .UseIdentityColumn();
        builder.Property(t => t.Title)
            .HasMaxLength(100);
        builder.Property(s => s.Summary)
            .HasMaxLength(1500);

    }
}
