using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Repositories;
using Application.Dtos;
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

public class SessionControllerIntegrationTests : IClassFixture<ControllersAppFactoryFixture>
{
    private readonly HttpClient _httpClient;
    private readonly ControllersAppFactoryFixture _controllersAppFactory;

    public SessionControllerIntegrationTests(ControllersAppFactoryFixture controllersAppFactory)
    {
        _controllersAppFactory = controllersAppFactory;
        _httpClient = _controllersAppFactory.CreateClient();
        
        var jwtTokenGeneratorService = new JwtTokenGeneratorService();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken(TestUtils.AdminId, "admin@mail.com", new List<string> { "Admin" }));
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
        content.Count().Should().Be(fakeData.Count);
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
        content.Expiry.Should().BeCloseTo(session.Expiry, TimeSpan.FromMicroseconds(100));
        content.Start.Should().BeCloseTo(session.Start, TimeSpan.FromMicroseconds(100));
        content.Product.Id.Should().Be(session.Product.Id);
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
}