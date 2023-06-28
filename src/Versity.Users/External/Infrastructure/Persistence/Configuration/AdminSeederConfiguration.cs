using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class AdminSeederConfiguration : IEntityTypeConfiguration<VersityUser>
{
    public void Configure(EntityTypeBuilder<VersityUser> builder)
    {
        var user = new VersityUser
        {
            Id = "4e274126-1d8a-4dfd-a025-806987095809",
            UserName = "Versity Admin",
            Email = Environment.GetEnvironmentVariable("ADMIN__Email"),
            NormalizedEmail = "versity.identity.dev@gmail.com".ToUpper(),
            EmailConfirmed = true,
            FirstName = "Versity",
            LastName = "Admin",
            SecurityStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "+000000000000"
        };
        
        var passwordHasher = new PasswordHasher<VersityUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, Environment.GetEnvironmentVariable("ADMIN__Password"));
        builder.HasData(user);
    }
}