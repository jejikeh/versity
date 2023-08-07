using Application.Abstractions;
using Infrastructure.Services;
using Infrastructure.Services.KafkaConsumer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Presentation.Configuration;
using Sessions.Tests.Integrations.Helpers;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Sessions.Tests.Integrations.Fixture;

public class ControllersAppFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly RedisContainer _redisContainer;
    private readonly KafkaContainer _kafkaContainer;
    
    public ControllersAppFactoryFixture()
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

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<KafkaConsumerConfiguration>();
            services.UseKafkaConsumer(new KafkaConsumerConfiguration());

            services.RemoveAll<IVersityUsersDataService>();
            services.AddScoped<IVersityUsersDataService, GrpcUsersServiceMock>();
        });
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
        await _kafkaContainer.StopAsync();
    }
}