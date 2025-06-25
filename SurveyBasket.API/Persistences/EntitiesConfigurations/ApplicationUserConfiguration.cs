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
        builder
            .OwnsMany(u => u.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        builder
            .HasData(new ApplicationUser
            {
                Id = DefaultUsers.AdminId,
                FirstName = DefaultUsers.AdminFirstName,
                LastName = DefaultUsers.AdminLastName,
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                PhoneNumber = DefaultUsers.AdminPhoneNumber,
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                PasswordHash = DefaultUsers.AdminPassword
            });

    }
}
