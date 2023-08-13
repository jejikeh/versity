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
        var user = new VersityUser
        {
            Id = _configuration["Admin:Id"] 
                 ?? throw new UserSecretsInvalidException("setup-admin-id-secret"),
            UserName = "Versity Admin",
            Email = _configuration["Admin:Email"] 
                    ?? throw new UserSecretsInvalidException("setup-admin-email-secret"),
            NormalizedEmail = _configuration["Admin:Email"]!.ToUpper(),
            NormalizedUserName = "VERSITY ADMIN",
            EmailConfirmed = true,
            FirstName = "Versity",
            LastName = "Admin",
            SecurityStamp = Guid.NewGuid().ToString("D"),
            PhoneNumber = "+000000000000"
        };
        
        var passwordHasher = new PasswordHasher<VersityUser>();
        user.PasswordHash = passwordHasher.HashPassword(
            user, 
            _configuration["Admin:Password"] ?? throw new UserSecretsInvalidException("setup-admin-password-secret"));
        
        builder.HasData(user);
    }
}