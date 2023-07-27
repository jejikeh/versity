using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Repositories;
using Application.Dtos;
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

public class UsersControllerIntegrationTests : IClassFixture<DockerWebApplicationFactoryFixture>
{
    private readonly DockerWebApplicationFactoryFixture _factory;
    private readonly HttpClient _httpClient;

    public UsersControllerIntegrationTests(DockerWebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        var jwtTokenGeneratorService = new JwtTokenGeneratorService(new TokenGenerationConfiguration());
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtTokenGeneratorService.GenerateToken("4e274126-1d8a-4dfd-a025-806987095809", "admin@mail.com", new List<string> { "Admin" }));
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

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Act
        var response = await _httpClient.GetAsync(HttpHelper.GetUserById("4e274126-1d8a-4dfd-a025-806987095809"));
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
    public async Task SetAdmin_ShouldGiveAdminRole_WhenUserExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var fakerGenerator = new Faker();
        var password = fakerGenerator.Internet.Password() + $"!{Utils.GenerateRandomString(3)}_$A";
        var userName = fakerGenerator.Internet.UserName();
        var userEmail = fakerGenerator.Internet.Email();
        var userPhone = fakerGenerator.Phone.PhoneNumber();
        var userFirstName = fakerGenerator.Name.FirstName();
        var lastName = fakerGenerator.Name.LastName();
        
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVersityUsersRepository>();
        repository?.CreateUserAsync(new Faker<VersityUser>().CustomInstantiator(faker => new VersityUser
        {
            Id = id.ToString(),
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            Email = userEmail,
            NormalizedEmail = userEmail.ToUpper(),
            EmailConfirmed = true,
            PhoneNumber = userPhone,
            FirstName = userFirstName,
            LastName = lastName
        }), password);
        
        // Act
        var response = await _httpClient.PostAsJsonAsync(HttpHelper.GiveAdminRoleToUser(id.ToString()), id);
        var token = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        
        // Assert
        response.EnsureSuccessStatusCode();
        token.Should().NotBeEmpty();
    }
}