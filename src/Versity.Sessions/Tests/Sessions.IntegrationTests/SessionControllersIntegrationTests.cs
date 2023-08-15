using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Domain.Models;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Infrastructure.Services.KafkaConsumer;
using Microsoft.Extensions.DependencyInjection;
using Sessions.IntegrationTests.Fixture;
using Sessions.IntegrationTests.Helpers;
using Sessions.Tests.Integrations.Helpers.Http;
using TestUtils = Sessions.IntegrationTests.Helpers.TestUtils;

namespace Sessions.IntegrationTests;

public class SessionControllersIntegrationTests : IClassFixture<ControllersAppFactoryFixture>, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ControllersAppFactoryFixture _controllersAppFactory;

    public SessionControllersIntegrationTests(ControllersAppFactoryFixture controllersAppFactory)
    {
        _controllersAppFactory = controllersAppFactory;
        _httpClient = _controllersAppFactory.CreateClient();
        
        var jwtTokenGeneratorService = new JwtTokenGeneratorService();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtTokenGeneratorService.GenerateToken(TestUtils.AdminId, "admin@mail.com", new List<string> { "Admin" }));
        
        using var scope = _controllersAppFactory.Services.CreateScope();
        var kafkaProductConsumerService = scope.ServiceProvider.GetService<KafkaProductConsumerService>();
        kafkaProductConsumerService?.StopAsync(CancellationToken.None);
    }
    
    [Fact]
    public async Task GetAllSessions_ShouldReturnSessions_WhenCommandIsValid()
    {
        // Arrange
        var fakeData = await SeedSessionsEntities();

        // Act
        var response = await _httpClient.GetAsync(SessionHttpHelper.GetAllSessions(1));
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<SessionViewModel>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
        content.Count().Should().BeGreaterOrEqualTo(fakeData.Count);
    }
    
    [Fact]
    public async Task GetAllProducts_ShouldReturnProducts_WhenCommandIsValid()
    {
        // Arrange
        var fakeData = await SeedSessionsEntities();

        // Act
        var response = await _httpClient.GetAsync(SessionHttpHelper.GetAllProducts(1));
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
        content.Count().Should().BeGreaterOrEqualTo(fakeData.Count);
    }
    
    [Fact]
    public async Task GetSessionById_ShouldReturnSessionModel_WhenCommandIsValid()
    {
        // Arrange
        var (session, _, _, _) = await SeedSessionEntities();

        // Act
        var response = await _httpClient.GetAsync(SessionHttpHelper.GetSessionById(session.Id.ToString()));
        var content = await response.Content.ReadFromJsonAsync<GetSessionByIdViewModel>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Id.Should().Be(session.Id);
        content.UserId.Should().Be(session.UserId);
        content.Expiry.ToUniversalTime().Should().BeCloseTo(session.Expiry.ToUniversalTime(), TimeSpan.FromSeconds(10));
        content.Start.ToUniversalTime().Should().BeCloseTo(session.Start.ToUniversalTime(), TimeSpan.FromSeconds(10));
        content.ProductId.Should().Be(session.ProductId);
    }
    
    [Fact]
    public async Task GetUserSessionsByUserId_ShouldReturnSessionModel_WhenCommandIsValid()
    {
        // Arrange
        var (session, _, _, _) = await SeedSessionEntities(Guid.Parse(TestUtils.AdminId));

        // Act
        var response = await _httpClient.GetAsync(SessionHttpHelper.GetUserSessionsByUserId(TestUtils.AdminId, 1));
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<UserSessionsViewModel>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Count().Should().BeGreaterOrEqualTo(1);
    }
    
    [Fact]
    public async Task GetAllProductSessions_ShouldReturnAllProducts_WhenCommandIsValid()
    {
        // Arrange
        var fakeData = await SeedSessionsEntities();

        // Act
        var response = await _httpClient.GetAsync(SessionHttpHelper.GetAllProducts(1));
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Count().Should().BeGreaterOrEqualTo(fakeData.Select(x => x.Item2).Count());
    }
    
    [Fact]
    public async Task CreateSession_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        var (_, product, _, _) = await SeedSessionEntities();
        var command = FakeDataGenerator.GenerateFakeCreateSessionCommand(Guid.NewGuid(), product.ExternalId);
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(SessionHttpHelper.CreateSession(), command);
        var content = await response.Content.ReadFromJsonAsync<SessionViewModel>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteSession_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        var (session, _, _, _) = await SeedSessionEntities();
        
        // Act
        var response = await _httpClient.DeleteAsync(SessionHttpHelper.DeleteSession(session.Id.ToString()));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task CloseSession_ShouldReturnOkAndCloseSession_WhenCommandIsValid()
    {
        // Arrange
        var (session, _, _, _) = await SeedSessionEntities();
        
        // Act
        var response = await _httpClient.PutAsJsonAsync(SessionHttpHelper.CloseSession(session.Id.ToString()), session.Id.ToString());
        var content = await response.Content.ReadFromJsonAsync<GetSessionByIdViewModel>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Status.Should().Be(SessionStatus.Closed);
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

    public void Dispose()
    {
        
    }

    private async Task<(Domain.Models.Session, Product, SessionLogs, List<LogData>)> SeedSessionEntities()
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
    
    private async Task<(Domain.Models.Session, Product, SessionLogs, List<LogData>)> SeedSessionEntities(Guid userId)
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
    
    private async Task<List<(Domain.Models.Session, Product, SessionLogs, List<LogData>)>> SeedSessionsEntities()
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
}