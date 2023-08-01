using Bogus;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Users.Tests.Integrations.Helpers;

public static class TestUtils
{
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