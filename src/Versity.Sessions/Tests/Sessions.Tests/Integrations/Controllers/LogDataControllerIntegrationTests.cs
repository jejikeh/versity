using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Repositories;
using Domain.Models;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Products.Tests.Integrations.Helpers;
using Sessions.Tests.Application;
using Sessions.Tests.Integrations.Fixture;
using Sessions.Tests.Integrations.Helpers;
using Sessions.Tests.Integrations.Helpers.Http;

namespace Sessions.Tests.Integrations.Controllers;

public class LogDataControllerIntegrationTests : IClassFixture<ControllersAppFactoryFixture>
{
    private readonly HttpClient _httpClient;
    private readonly ControllersAppFactoryFixture _controllersAppFactory;

    public LogDataControllerIntegrationTests(ControllersAppFactoryFixture controllersAppFactory)
    {
        _controllersAppFactory = controllersAppFactory;
        _httpClient = _controllersAppFactory.CreateClient();
        
        var jwtTokenGeneratorService = new JwtTokenGeneratorService();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken(TestUtils.AdminId, "admin@mail.com", new List<string> { "Admin" }));
    }

    [Fact]
    public async Task GetAllLogData_ShouldReturnLogDatas_WhenCommandIsValid()
    {
        // Arrange
        var fakeData = await SeedSessionsEntities();

        // Act
        var response = await _httpClient.GetAsync(LogDataHttpHelper.GetAllLogsData(1));
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<LogData>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
        content.Count().Should().BeGreaterOrEqualTo(fakeData.Count);
    }
    
    [Fact]
    public async Task GetLogDataById_ShouldReturnLogData_WhenCommandIsValid()
    {
        // Arrange
        var (_, _, _, logDatas) = await SeedSessionEntities();
        var logData = logDatas.First();

        // Act
        var response = await _httpClient.GetAsync(LogDataHttpHelper.GetLogDataById(logData.Id.ToString()));
        var content = await response.Content.ReadFromJsonAsync<LogData>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Id.Should().Be(logData.Id);
        content.Data.Should().Be(logData.Data);
        content.LogLevel.Should().Be(logData.LogLevel);
    }
    
    [Fact]
    public async Task CreateLogData_ShouldReturnLogData_WhenCommandIsValid()
    {
        // Arrange
        var (_, _, sessionLogs, _) = await SeedSessionEntities();
        var command = FakeDataGenerator.GenerateFakeCreateLogDataCommand(sessionLogs.Id);
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(LogDataHttpHelper.CreateLogData(), command);
        var content = await response.Content.ReadFromJsonAsync<LogData>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Data.Should().Be(command.Data);
        content.LogLevel.Should().Be(command.LogLevel);
    }
    
    [Fact]
    public async Task CreateLogsData_ShouldReturnLogData_WhenCommandIsValid()
    {
        // Arrange
        var (_, _, sessionLogs, _) = await SeedSessionEntities();
        var randomCount = Random.Shared.Next(1, 10);
        var command = FakeDataGenerator.GenerateFakeCreateLogDataDto(randomCount);
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(LogDataHttpHelper.CreateLogsData(sessionLogs.Id.ToString()), command);
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<LogData>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Count().Should().BeGreaterOrEqualTo(randomCount);
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
    
    private async Task<(Session, Product, SessionLogs, List<LogData>)> SeedSessionEntities()
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
            productsRepository);

        return (session, product, sessionLogs, logDatas);
    }
}