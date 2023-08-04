using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Sessions.Tests.Integrations.Helpers;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Sessions.Tests.Integrations.Fixture;

public class ControllersAppFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
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
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}