using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;
using Bogus;
using Domain.Models;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services.TokenServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Presentation.Configuration;
using Users.Tests.Integrations.Fixtures;
using Users.Tests.Integrations.Helpers;
using Xunit.Priority;
using Utils = Application.Common.Utils;

namespace Users.Tests.Integrations.Controllers;

[Collection("Integration Tests")]
public class UsersControllerIntegrationTests : IClassFixture<ControllersAppFactoryFixture>
{
    private readonly HttpClient _httpClient;
    private readonly ControllersAppFactoryFixture _controllersAppFactory;

    public UsersControllerIntegrationTests(ControllersAppFactoryFixture factoryUsersController)
    {
        _controllersAppFactory = factoryUsersController;
        _httpClient = factoryUsersController.CreateClient();

        using var scope = _controllersAppFactory.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetService<IConfiguration>();
        var jwtTokenGeneratorService = new JwtTokenGeneratorService(new TokenGenerationConfiguration(configuration));
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken(
            TestUtils.AdminId, 
            TestUtils.AdminEmail, 
            new List<string> { "Admin" }));
    }
    
    [Fact]
    public async Task GetAllUsers_ShouldReturnUsers_WhenUsersExists()
    {
        // Act
        var response = await _httpClient.GetAsync(HttpHelper.GetAllUsersUrl(1));
        
        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Act
        var response = await _httpClient.GetAsync(HttpHelper.GetUserById(TestUtils.AdminId));
        var result  = await response.Content.ReadFromJsonAsync<ViewVersityUserDto>();
        
        // Assert
        response.EnsureSuccessStatusCode();
        result.Should().NotBeNull();
        result.Role.Should().Contain("Admin");
        result.Email.Should().Be(TestUtils.AdminEmail);
    }

    [Fact]
    public async Task ChangeUserPassword_ShouldReturnValidationError_WhenPasswordTooShort()
    {
        // Arrange
        var command = new ChangeUserPasswordDto("versity.Adm1n.dev-31_13%versity", "new_admin");
        
        // Act
        var response = await _httpClient.PutAsJsonAsync(HttpHelper.ChangeUserPassword("4e274126-1d8a-4dfd-a025-806987095809"), command);
        
        // Act
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task ChangeUserPassword_ShouldChangePassword_WhenPasswordIsStrong()
    {
        // Arrange
        using var scope = _controllersAppFactory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetService<IVersityUsersRepository>();
        var (user, password) = await VersityUserSeeder.SeedUserDataAsync(repository);
        var faker = new Faker();
        var command = new ChangeUserPasswordDto(
            password,
            faker.Internet.Password(5) + $"!{Utils.GenerateRandomString(2)}!p1A2");
        
        // Act
        var response = await _httpClient.PutAsJsonAsync(HttpHelper.ChangeUserPassword(user.Id), command);
        
        // Act
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task SetAdmin_ShouldReturn404_WhenUserDoesntExists()
    {
        // Arrange
        var id = Guid.NewGuid();
     
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.GiveAdminRoleToUser(id.ToString()), id);
        
        // Act
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task SetAdmin_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        using var scope = _controllersAppFactory.Services.CreateScope();
        var versityUsersRepository = scope.ServiceProvider.GetService<IVersityUsersRepository>();
        var (user, password) = await VersityUserSeeder.SeedUserDataAsync(versityUsersRepository);
     
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.GiveAdminRoleToUser(user.Id), password);
        
        // Act
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}