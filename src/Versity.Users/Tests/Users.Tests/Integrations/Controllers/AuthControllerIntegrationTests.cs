using System.Net;
using System.Net.Http.Json;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Bogus;
using FluentAssertions;
using Infrastructure.Services.TokenServices;
using Microsoft.AspNetCore.Identity;
using Presentation.Configuration;
using Users.Tests.Integrations.Fixtures;
using Users.Tests.Integrations.Helpers;
using Utils = Application.Common.Utils;

namespace Users.Tests.Integrations.Controllers;

public class AuthControllerIntegrationTests : IClassFixture<WebAppFactoryFixture>
{
    private readonly HttpClient _httpClient;

    public AuthControllerIntegrationTests(WebAppFactoryFixture factoryUsersController)
    {
        _httpClient = factoryUsersController.CreateClient();
        
        var jwtTokenGeneratorService = new JwtTokenGeneratorService(new TokenGenerationConfiguration());
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken("4e274126-1d8a-4dfd-a025-806987095809", "admin@mail.com", new List<string> { "Admin" }));
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
    public async Task Login_ShouldReturnUnauthorized_WhenModelIsValid()
    {
        // Arrange
        var command = GenerateFakeLoginVersityUserCommand();
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.Login(), command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Login_ShouldReturnOk_WhenModelIsValid()
    {
        // Arrange
        var command = new LoginVersityUserCommand(TestUtils.UserEmail, TestUtils.UserPassword);
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.Login(), command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static RegisterVersityUserCommand GenerateInvalidRegisterVersityUserCommand()
    {
        return new Faker<RegisterVersityUserCommand>().CustomInstantiator(faker => 
            new RegisterVersityUserCommand(
                faker.Name.FirstName(), 
                faker.Name.LastName(), 
                faker.Internet.Email(), 
                faker.Phone.PhoneNumber(), 
                faker.Internet.Password() + $"!{Utils.GenerateRandomString(4)}"))
            .Generate();
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
    
    private LoginVersityUserCommand GenerateFakeLoginVersityUserCommand()
    {
        return new Faker<LoginVersityUserCommand>().CustomInstantiator(faker => new LoginVersityUserCommand(
                faker.Internet.Email(),
                faker.Internet.Password()))
            .Generate();
    }
    
    private static ConfirmEmailCommand GenerateFakeConfirmEmailCommand()
    {
        return new Faker<ConfirmEmailCommand>().CustomInstantiator(faker => new ConfirmEmailCommand(
                faker.Random.Guid().ToString(),
                faker.Random.Guid().ToString()))
            .Generate();
    }
}