using Bogus;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Users.Tests.Integrations.Helpers;

public static class TestUtils
{
    public static string UserId = "847998fe-7b66-4ef9-8a2e-f56baf59ed4f";
    public static string UserPassword = "123!@#OIFJRS$jfsio";
    public static string UserEmail = "test@mail.com";

    
    public static void SeedUserData(VersityUsersDbContext context)
    {
        var user = new VersityUser
        {
            Id = "UserId",
            UserName = "Versity Test",
            Email = "test@mail.com",
            NormalizedEmail = "test@gmail.com".ToUpper(),
            EmailConfirmed = true,
            FirstName = "Versity",
            LastName = "Test",
            SecurityStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "+000000000000"
        };
        
        var passwordHasher = new PasswordHasher<VersityUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, UserPassword);
        
        context.Users.Add(user);
    }
    
    public static void SeedEnvironmentVariables(string dbConnectionString, string redisConnectionString, string elasticsearchConnectionString)
    {
        Environment.SetEnvironmentVariable("ConnectionString", dbConnectionString);
        Environment.SetEnvironmentVariable("REDIS_Host", redisConnectionString);
        Environment.SetEnvironmentVariable("ElasticConfiguration:Uri", elasticsearchConnectionString);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("ADMIN__Password", "admin");
        Environment.SetEnvironmentVariable("ADMIN__Email", "admin@mail.com");
        Environment.SetEnvironmentVariable("JWT__Issuer", "testing");
        Environment.SetEnvironmentVariable("JWT__Audience", "testing");
        Environment.SetEnvironmentVariable("JWT__Key", Guid.NewGuid().ToString());
    }
}