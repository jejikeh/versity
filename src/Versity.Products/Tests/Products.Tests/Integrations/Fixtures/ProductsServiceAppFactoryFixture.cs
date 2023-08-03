using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Products.Tests.Integrations.Helpers;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Products.Tests.Integrations.Fixtures;

public class ProductsServiceAppFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly RedisContainer _redisContainer;
    private readonly KafkaContainer _kafkaContainer;
    
    public ProductsServiceAppFactoryFixture()
    {
        _dbContainer = new PostgreSqlBuilder().Build();
        _redisContainer = new RedisBuilder().Build();
        _kafkaContainer = new KafkaBuilder().Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        TestUtils.SeedEnvironmentVariables(
            _dbContainer.GetConnectionString(),
            _redisContainer.GetConnectionString(),
            _kafkaContainer.GetBootstrapAddress());
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory());    
        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _kafkaContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _kafkaContainer.StartAsync();
    }
}