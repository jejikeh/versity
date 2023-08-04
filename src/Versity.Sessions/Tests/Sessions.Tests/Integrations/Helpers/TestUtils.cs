﻿namespace Sessions.Tests.Integrations.Helpers;

public static class TestUtils
{
    public const string AdminId = "4e274126-1d8a-4dfd-a025-806987095809";
    
    public static void SeedEnvironmentVariables(string dbConnectionString, string redisConnectionString, string elasticSearchConnectionString = "no_set")
    {
        Environment.SetEnvironmentVariable("ConnectionString", dbConnectionString);
        Environment.SetEnvironmentVariable("REDIS_Host", redisConnectionString);
        Environment.SetEnvironmentVariable("ElasticConfiguration:Uri", elasticSearchConnectionString);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("ADMIN__Password", "admin");
        Environment.SetEnvironmentVariable("ADMIN__Email", "admin@mail.com");
        Environment.SetEnvironmentVariable("JWT__Issuer", "testing");
        Environment.SetEnvironmentVariable("JWT__Audience", "testing");
        Environment.SetEnvironmentVariable("JWT__Key", Guid.NewGuid().ToString());
    }
}