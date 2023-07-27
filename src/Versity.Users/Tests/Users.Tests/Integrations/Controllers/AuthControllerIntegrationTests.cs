using System.Net;
using System.Net.Http.Json;
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

public class AuthControllerIntegrationTests : IClassFixture<DockerWebApplicationFactoryFixture>
{
    private readonly DockerWebApplicationFactoryFixture _factory;
    private readonly HttpClient _httpClient;

    public AuthControllerIntegrationTests(DockerWebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        
        var jwtTokenGeneratorService = new JwtTokenGeneratorService(new TokenGenerationConfiguration());
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken("4e274126-1d8a-4dfd-a025-806987095809", "admin@mail.com", new List<string> { "Admin" }));
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenModelIsInvalid()
    {
        // Arrange
        var command = GenerateInvalidRegisterVersityUserCommand();
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.Register(), command);
        
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
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
                    GenerateValidPhoneNumber(), 
                    faker.Internet.Password() + $"!{Utils.GenerateRandomString(4)}"))
            .Generate();
    }

    private static string GenerateValidPhoneNumber()
    {
        return $"+375333{new Random().Next(100000, 999999)}";
    }
}