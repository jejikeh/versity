namespace Products.Tests.Integrations.Helpers;

public static class TestUtils
{
    public const string AdminId = "4e274126-1d8a-4dfd-a025-806987095809";
    
    public static void SeedEnvironmentVariables(
        string dbConnectionString, 
        string redisConnectionString, 
        string kafkaConnectionString,
        string elasticsearchConnectionString = "no_set")
    {
        Environment.SetEnvironmentVariable("ConnectionString", dbConnectionString);
        Environment.SetEnvironmentVariable("REDIS_Host", redisConnectionString);
        Environment.SetEnvironmentVariable("ElasticConfiguration:Uri", elasticsearchConnectionString);
        Environment.SetEnvironmentVariable("KAFKA_Host", kafkaConnectionString);
        Environment.SetEnvironmentVariable("KAFKA_ProducerName", "versity.products.tests");
        Environment.SetEnvironmentVariable("KAFKA_Topic", "versity.products");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("JWT__Issuer", "testing");
        Environment.SetEnvironmentVariable("JWT__Audience", "testing");
        Environment.SetEnvironmentVariable("JWT__Key", Guid.NewGuid().ToString());
    }
}