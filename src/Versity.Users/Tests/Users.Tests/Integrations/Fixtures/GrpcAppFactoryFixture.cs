using Application.Abstractions.Repositories;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Presentation.bin;
using Testcontainers.Elasticsearch;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Users.Tests.Integrations.Helpers;
using Users.Tests.Integrations.Helpers.Mocks;

namespace Users.Tests.Integrations.Fixtures;

public class GrpcAppFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly RedisContainer _redisContainer;
    private readonly ElasticsearchContainer _elasticsearchContainer;

    public GrpcAppFactoryFixture()
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

        GrpcChannel grpcChannel = null;
        var client = CreateDefaultClient(new ResponseVersionHandler());
        if (client.BaseAddress is not null)
        {
            grpcChannel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });
        }
        
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ISmtpClientService>();
            services.AddTransient<ISmtpClientService, SmtpClientServiceMock>();
            services.AddSingleton(grpcChannel);
            services.AddScoped<IGrpcUsersDataServiceMock, GrpcUsersDataServiceMock>();
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
    
    private class ResponseVersionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            response.Version = request.Version;
            return response;
        }
    }
}