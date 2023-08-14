namespace Sessions.IntegrationTests.Helpers;

public static class TestUtils
{
    public const string AdminId = "4e274126-1d8a-4dfd-a025-806987095809";
    
    public static void SeedEnvironmentVariables(
        string dbConnectionString, 
        string redisConnectionString, 
        string kafkaConnectionString = "no_set",
        string elasticsearchConnectionString = "no_set")
    {
        Environment.SetEnvironmentVariable("TEST_ConnectionString", dbConnectionString);
        Environment.SetEnvironmentVariable("TEST_CacheHost", redisConnectionString);
        Environment.SetEnvironmentVariable("TEST_DatabaseName", "versity_sessions");
        Environment.SetEnvironmentVariable("ElasticConfiguration:Uri", elasticsearchConnectionString);
        Environment.SetEnvironmentVariable("KAFKA_Host", kafkaConnectionString);
        Environment.SetEnvironmentVariable("KAFKA_ProducerName", "versity.products.tests");
        Environment.SetEnvironmentVariable("KAFKA_Topic", "versity.products");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
        Environment.SetEnvironmentVariable("JWT__Issuer", "testing");
        Environment.SetEnvironmentVariable("JWT__Audience", "testing");
        Environment.SetEnvironmentVariable("JWT__Key", Guid.NewGuid().ToString());
    }
}