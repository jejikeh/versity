using Application.Abstractions;
using DotNet.Testcontainers.Images;
using Infrastructure.Services.KafkaConsumer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Presentation.Configuration;
using Sessions.IntegrationTests.Helpers;
using Sessions.Tests.Integrations.Helpers;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Sessions.IntegrationTests.Fixture
{
    public class ControllersAppFactoryFixture : WebApplicationFactory<Presentation.Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        private readonly RedisContainer _redisContainer;
    
        public ControllersAppFactoryFixture()
        {
            _dbContainer = new PostgreSqlBuilder().Build();
            _redisContainer = new RedisBuilder().Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            TestUtils.SeedEnvironmentVariables(
                _dbContainer.GetConnectionString(),
                _redisContainer.GetConnectionString());

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IVersityUsersDataService>();
                services.AddScoped<IVersityUsersDataService, GrpcUsersServiceMock>();
            });
        }
    
        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            await _redisContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _redisContainer.StopAsync();
        }
    }
}