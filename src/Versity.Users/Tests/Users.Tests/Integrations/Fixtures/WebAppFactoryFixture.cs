using System.Reflection;
using Application.Abstractions.Repositories;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure.Persistence;
using Infrastructure.Services.EmailServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Presentation.bin;
using Testcontainers.Elasticsearch;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Users.Tests.Integrations.Helpers;

namespace Users.Tests.Integrations.Fixtures;

public class WebAppFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly RedisContainer _redisContainer;
    private readonly ElasticsearchContainer _elasticsearchContainer;
    
    public WebAppFactoryFixture()
    {
        _dbContainer = new PostgreSqlBuilder().Build();
        _redisContainer = new RedisBuilder().Build();
        _elasticsearchContainer = new ElasticsearchBuilder().Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        TestUtils.SeedEnvironmentVariables(
            _dbContainer.GetConnectionString(),
            _redisContainer.GetConnectionString(),
            _elasticsearchContainer.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ISmtpClientService>();
            services.AddTransient<ISmtpClientService, SmtpClientServiceMock>();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _elasticsearchContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _elasticsearchContainer.StopAsync();
    }
}