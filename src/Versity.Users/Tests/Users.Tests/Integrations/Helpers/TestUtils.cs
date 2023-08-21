using Bogus;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Users.Tests.Integrations.Helpers;

public static class TestUtils
{
    public const string AdminId = "4e274126-1d8a-4dfd-a025-806987095809";
    public const string AdminEmail = "versity.identity.dev@gmail.com";
    
    public static void SeedEnvironmentVariables(string dbConnectionString, string redisConnectionString, string elasticSearchConnectionString = "no_set")
    {
        Environment.SetEnvironmentVariable("TEST_ConnectionString", dbConnectionString);
        Environment.SetEnvironmentVariable("TEST_CacheHost", redisConnectionString);
        Environment.SetEnvironmentVariable("ElasticConfiguration:Uri", elasticSearchConnectionString);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
    }
}