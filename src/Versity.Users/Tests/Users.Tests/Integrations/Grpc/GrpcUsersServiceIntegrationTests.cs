using Application.Abstractions.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Users.Tests.Integrations.Fixtures;
using Users.Tests.Integrations.Helpers;
using Users.Tests.Integrations.Helpers.Mocks;

namespace Users.Tests.Integrations.Grpc;

public class GrpcUsersServiceIntegrationTests : IClassFixture<GrpcAppFactoryFixture>
{
    private readonly GrpcAppFactoryFixture _grpcAppFactoryFixture;

    public GrpcUsersServiceIntegrationTests(GrpcAppFactoryFixture grpcAppFactoryFixture)
    {
        _grpcAppFactoryFixture = grpcAppFactoryFixture;
    }

    [Fact]
    public async Task IsUserExist_ShouldReturnFalse_WhenUserDoesntExists()
    {
        // Arrange
        using var scope = _grpcAppFactoryFixture.Services.CreateScope();
        var grpcUsersDataServiceMock = scope.ServiceProvider.GetService<IGrpcUsersDataServiceMock>();
        
        // Act
        var result = await grpcUsersDataServiceMock.IsUserExistAsync(Guid.NewGuid().ToString());
        
        // Arrange
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task IsUserExist_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        using var scope = _grpcAppFactoryFixture.Services.CreateScope();
        var repository = scope.ServiceProvider.GetService<IVersityUsersRepository>();
        var (user, _) = await VersityUserSeeder.SeedUserDataAsync(repository);
        var grpcUsersDataServiceMock = scope.ServiceProvider.GetService<IGrpcUsersDataServiceMock>();
        
        // Act
        var result = await grpcUsersDataServiceMock.IsUserExistAsync(user.Id);
        
        // Arrange
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetUserRolesAsync_ShouldReturnArray_WhenUserExists()
    {
        // Arrange
        using var scope = _grpcAppFactoryFixture.Services.CreateScope();
        var grpcUsersDataServiceMock = scope.ServiceProvider.GetService<IGrpcUsersDataServiceMock>();
        
        // Act
        var result = await grpcUsersDataServiceMock.GetUserRolesAsync(TestUtils.AdminId);
        
        // Arrange
        result.Should().Contain("Admin");
    }
}