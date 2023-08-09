using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Repositories;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Services.TokenServices;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Configuration;
using Users.Tests.Integrations.Fixtures;
using Users.Tests.Integrations.Helpers;
using Utils = Application.Common.Utils;

namespace Users.Tests.Integrations.Controllers;

public class AuthControllerIntegrationTests : IClassFixture<ControllersAppFactoryFixture>
{
    private readonly HttpClient _httpClient;
    private readonly ControllersAppFactoryFixture _controllersAppControllersAppFactory;

    public AuthControllerIntegrationTests(ControllersAppFactoryFixture controllersAppFactoryFixture)
    {
        _controllersAppControllersAppFactory = controllersAppFactoryFixture;
        _httpClient = controllersAppFactoryFixture.CreateClient();
        
        var jwtTokenGeneratorService = new JwtTokenGeneratorService(new TokenGenerationConfiguration());
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken(TestUtils.AdminId, "admin@mail.com", new List<string> { "Admin" }));
    }

    [Fact]
    public async Task ConfirmEmail_ShouldReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var code = Guid.NewGuid().ToString();

        // Act
        var response = await _httpClient.GetAsync(HttpHelper.ConfirmEmail(id, code));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Login_ShouldReturnOk_WhenModelIsValid()
    {
        // Arrange
        using var scope = _controllersAppControllersAppFactory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetService<IVersityUsersRepository>();
        var (user, password) = await VersityUserSeeder.SeedUserDataAsync(repository);
        var command = new LoginVersityUserCommand(user.Email, password);
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.Login(), command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenModelIsValid()
    {
        // Arrange
        var command = new LoginVersityUserCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.Login(), command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Register_ShouldReturnOk_WhenModelIsValid()
    {
        // Arrange
        var command = GenerateRegisterVersityUserCommand();
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.Register(), command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task ResendEmailVerificationToken_ShouldReturnError_WhenEmailIsAlreadyVerified()
    {
        // Arrange
        using var scope = _controllersAppControllersAppFactory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetService<IVersityUsersRepository>();
        var (user, password) = await VersityUserSeeder.SeedUserDataAsync(repository);
        var command = new ResendEmailVerificationTokenCommand(user.Email, password);
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.ResendEmailVerificationToken(), command);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.InternalServerError);
    }
    
    [Fact]
    public async Task RefreshToken_ShouldReturnError_WhenUserDoesntExists()
    {
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.RefreshJwtToken(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()), Guid.NewGuid().ToString());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
    
    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        
        using var scope = _controllersAppControllersAppFactory.Services.CreateScope();
        var versityUsersRepository = scope.ServiceProvider.GetService<IVersityUsersRepository>();
        var versityRefreshTokensRepository = scope.ServiceProvider.GetService<IVersityRefreshTokensRepository>();
        
        var (user, _) = await VersityUserSeeder.SeedUserDataAsync(versityUsersRepository);
        await versityRefreshTokensRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = token,
            IsUsed = false,
            IsRevoked = false,
            AddedTime = DateTime.UtcNow,
            ExpiryTime = DateTime.UtcNow.AddHours(1)
        }, CancellationToken.None);
        await versityRefreshTokensRepository.SaveChangesAsync(CancellationToken.None);
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.RefreshJwtToken(user.Id, token), user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static RegisterVersityUserCommand GenerateRegisterVersityUserCommand()
    {
        return new Faker<RegisterVersityUserCommand>().CustomInstantiator(faker => 
                new RegisterVersityUserCommand(
                    faker.Name.FirstName(), 
                    faker.Name.LastName(), 
                    faker.Internet.Email(), 
                    "+37533322222", 
                    faker.Internet.Password() + $"!{Utils.GenerateRandomString(4)}"))
            .Generate();
    }
}