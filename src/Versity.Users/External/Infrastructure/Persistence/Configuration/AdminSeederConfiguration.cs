using Domain.Models;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Configuration;

public class AdminSeederConfiguration : IEntityTypeConfiguration<VersityUser>
{
    private readonly IConfiguration _configuration;

    public AdminSeederConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(EntityTypeBuilder<VersityUser> builder)
    {
        string id, email, password;
        
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEST_ConnectionString")))
        {
            id = "4e274126-1d8a-4dfd-a025-806987095809";
            email = "versity.identity.dev@gmail.com";
            password = "versity.Adm1n.dev-31_13%versity";
        }
        else
        {
            id = _configuration["Admin:Id"] ?? throw new UserSecretsInvalidException("setup-admin-id-secret");
            email = _configuration["Admin:Email"] ?? throw new UserSecretsInvalidException("setup-admin-email-secret");
            password = _configuration["Admin:Password"] ?? throw new UserSecretsInvalidException("setup-admin-password-secret");
        }
        
        var user = new VersityUser
        {
            Id = id,
            UserName = "Versity Admin",
            Email = email,
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = "VERSITY ADMIN",
            EmailConfirmed = true,
            FirstName = "Versity",
            LastName = "Admin",
            SecurityStamp = Guid.NewGuid().ToString("D"),
            PhoneNumber = "+000000000000"
        };
        
        var passwordHasher = new PasswordHasher<VersityUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, password);
        builder.HasData(user);
    }
}