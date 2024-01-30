using Application.Abstractions.Repositories;
using FluentAssertions;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Users.Tests.Integrations.Fixtures;
using Users.Tests.Integrations.Helpers;
using Users.Tests.Integrations.Helpers.Mocks;

namespace Users.Tests.Integrations.Grpc;

public class GrpcUsersServiceIntegrationTests : IClassFixture<GrpcAppFactoryFixture>
{
    private readonly GrpcAppFactoryFixture _grpcAppFactoryFixture;
    private readonly GrpcChannel? _grpcChannel;

    public GrpcUsersServiceIntegrationTests(GrpcAppFactoryFixture grpcAppFactoryFixture)
    {
        _grpcAppFactoryFixture = grpcAppFactoryFixture;
        
        var client = _grpcAppFactoryFixture.CreateClient();
        if (client.BaseAddress is not null)
        {
            _grpcChannel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });
        }
    }

    [Fact]
    public async Task IsUserExist_ShouldReturnFalse_WhenUserDoesntExists()
    {
        // Act
        var result = await _grpcChannel?.IsUserExistAsync(Guid.NewGuid().ToString());
        
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
        
        // Act
        var result = await _grpcChannel?.IsUserExistAsync(user.Id);
        
        // Arrange
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetUserRolesAsync_ShouldReturnArray_WhenUserExists()
    {
        // Arrange
        using var scope = _grpcAppFactoryFixture.Services.CreateScope();
        
        // Act
        var result = await _grpcChannel?.GetUserRolesAsync(TestUtils.AdminId);
        
        // Arrange
        result.Should().Contain("Admin");
    }
}