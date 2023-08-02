using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Bogus;
using Domain.Models;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services.TokenServices;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Configuration;
using Users.Tests.Integrations.Fixtures;
using Users.Tests.Integrations.Helpers;
using Xunit.Priority;
using Utils = Application.Common.Utils;

namespace Users.Tests.Integrations.Controllers;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UsersControllerIntegrationTests : IClassFixture<ControllersAppFactoryFixture>
{
    private readonly HttpClient _httpClient;
    private readonly ControllersAppFactoryFixture _controllersAppFactory;

    public UsersControllerIntegrationTests(ControllersAppFactoryFixture factoryUsersController)
    {
        _controllersAppFactory = factoryUsersController;
        _httpClient = factoryUsersController.CreateClient();
        var jwtTokenGeneratorService = new JwtTokenGeneratorService(new TokenGenerationConfiguration());
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken(TestUtils.AdminId, "admin@mail.com", new List<string> { "Admin" }));
    }
    
    [Fact]
    public async Task GetAllUsers_ShouldReturnUsers_WhenUsersExists()
    {
        // Act
        var response = await _httpClient.GetAsync(HttpHelper.GetAllUsersUrl(1));
        var result  = await response.Content.ReadFromJsonAsync<IEnumerable<ViewVersityUserDto>>();
        
        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact, Priority(-10)]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Act
        var response = await _httpClient.GetAsync(HttpHelper.GetUserById(TestUtils.AdminId));
        var result  = await response.Content.ReadFromJsonAsync<ViewVersityUserDto>();
        
        // Assert
        response.EnsureSuccessStatusCode();
        result.Should().NotBeNull();
        result.Role.Should().Contain("Admin");
        result.Email.Should().Be("admin@mail.com");
    }

    [Fact]
    public async Task ChangeUserPassword_ShouldReturnValidationError_WhenPasswordTooShort()
    {
        // Arrange
        var command = new ChangeUserPasswordDto("admin", "new_admin");
        
        // Act
        var response = await _httpClient.PutAsJsonAsync(HttpHelper.ChangeUserPassword("4e274126-1d8a-4dfd-a025-806987095809"), command);
        
        // Act
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task ChangeUserPassword_ShouldChangePassword_WhenPasswordIsStrong()
    {
        // Arrange
        var command = new ChangeUserPasswordDto("admin", "neW_admin!123");
        
        // Act
        var response = await _httpClient.PutAsJsonAsync(HttpHelper.ChangeUserPassword("4e274126-1d8a-4dfd-a025-806987095809"), command);
        
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