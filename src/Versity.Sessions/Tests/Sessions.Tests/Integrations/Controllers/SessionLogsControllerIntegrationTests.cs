using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Repositories;
using Domain.Models;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Products.Tests.Integrations.Helpers;
using Sessions.Tests.Integrations.Fixture;
using Sessions.Tests.Integrations.Helpers;
using Sessions.Tests.Integrations.Helpers.Http;

namespace Sessions.Tests.Integrations.Controllers;

public class SessionLogsControllerIntegrationTests : IClassFixture<ControllersAppFactoryFixture>
{
    private readonly HttpClient _httpClient;
    private readonly ControllersAppFactoryFixture _controllersAppFactory;
    
    public SessionLogsControllerIntegrationTests(ControllersAppFactoryFixture controllersAppFactory)
    {
        _controllersAppFactory = controllersAppFactory;
        _httpClient = _controllersAppFactory.CreateClient();
        
        var jwtTokenGeneratorService = new JwtTokenGeneratorService();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken(TestUtils.AdminId, "admin@mail.com", new List<string> { "Admin" }));
    }

    [Fact]
    public async Task GetAllSessionLogs_ShouldReturnSessionsLogs_WhenCommandIsValid()
    {
        // Arrange
        var fakeData = await SeedSessionsEntities();
        
        // Act
        var response = await _httpClient.GetAsync(SessionLogsHttpHelper.GetSessionsLogs(1));
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<SessionLogs>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
        content.Count().Should().BeGreaterOrEqualTo(fakeData.Select(x => x.Item3).Count());
    }
    
    [Fact]
    public async Task GetSessionLogsById_ShouldReturnSessionsLogs_WhenCommandIsValid()
    {
        // Arrange
        var (_, _, sessionLogs, _) = await SeedSessionEntities(Guid.Parse(TestUtils.AdminId));
        
        // Act
        var response = await _httpClient.GetAsync(SessionLogsHttpHelper.GetSessionLogsById(sessionLogs.Id.ToString()));
        var content = await response.Content.ReadFromJsonAsync<SessionLogs>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Id.Should().Be(sessionLogs.Id);
        content.SessionId.Should().Be(sessionLogs.SessionId);
    }
    
    private async Task<List<(Session, Product, SessionLogs, List<LogData>)>> SeedSessionsEntities()
    {
        using var scope = _controllersAppFactory.Services.CreateScope();
        var sessionRepository = scope.ServiceProvider.GetService<ISessionsRepository>();
        var sessionLogsRepository = scope.ServiceProvider.GetService<ISessionLogsRepository>();
        var logDataRepository = scope.ServiceProvider.GetService<ILogsDataRepository>();
        var productsRepository = scope.ServiceProvider.GetService<IProductsRepository>();

        var fakeData = await SessionSeeder.SeedSessionsDataAsync(
            sessionRepository,
            sessionLogsRepository,
            logDataRepository,
            productsRepository);

        return fakeData;
    }
    
    private async Task<(Session, Product, SessionLogs, List<LogData>)> SeedSessionEntities(Guid userId)
    {
        using var scope = _controllersAppFactory.Services.CreateScope();
        var sessionRepository = scope.ServiceProvider.GetService<ISessionsRepository>();
        var sessionLogsRepository = scope.ServiceProvider.GetService<ISessionLogsRepository>();
        var logDataRepository = scope.ServiceProvider.GetService<ILogsDataRepository>();
        var productsRepository = scope.ServiceProvider.GetService<IProductsRepository>();

        var (session, product, sessionLogs, logDatas) = await SessionSeeder.SeedSessionDataAsync(
            sessionRepository,
            sessionLogsRepository,
            logDataRepository,
            productsRepository,
            userId);

        return (session, product, sessionLogs, logDatas);
    }
}