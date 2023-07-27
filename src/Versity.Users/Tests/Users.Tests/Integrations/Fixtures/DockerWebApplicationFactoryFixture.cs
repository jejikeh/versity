using System.Reflection;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using Testcontainers.Elasticsearch;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Users.Tests.Integrations.Fixtures;

public class DockerWebApplicationFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly RedisContainer _redisContainer;
    private readonly ElasticsearchContainer _elasticsearchContainer;
    
    public DockerWebApplicationFactoryFixture()
    {
        _dbContainer = new PostgreSqlBuilder().Build();
        _redisContainer = new RedisBuilder().Build();
        _elasticsearchContainer = new ElasticsearchBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _elasticsearchContainer.StartAsync();
        
        Environment.SetEnvironmentVariable("ConnectionString", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("REDIS_Host", _redisContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ElasticConfiguration:Uri", _elasticsearchContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("ADMIN__Password", "admin");
        Environment.SetEnvironmentVariable("ADMIN__Email", "admin@mail.com");
        Environment.SetEnvironmentVariable("JWT__Issuer", "testing");
        Environment.SetEnvironmentVariable("JWT__Audience", "testing");
        Environment.SetEnvironmentVariable("JWT__Key", Guid.NewGuid().ToString());
        
        using var scope = Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<VersityUsersDbContext>();
            
        await context.Database.EnsureCreatedAsync();
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _elasticsearchContainer.StopAsync();
    }
}