using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Services.TokenServices;
using Moq;

namespace Users.Tests.Infrastructure.Services.TokenServices;

public class RefreshTokenGeneratorServiceTests
{
    private readonly Mock<IVersityRefreshTokensRepository> _refreshTokensRepository;
    private readonly RefreshTokenGeneratorService _refreshTokenGeneratorService;

    public RefreshTokenGeneratorServiceTests()
    {
        _refreshTokensRepository = new Mock<IVersityRefreshTokensRepository>();
        _refreshTokenGeneratorService = new RefreshTokenGeneratorService(_refreshTokensRepository.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken_WhenCalled()
    {
        // Arrange
        var userId = new Guid().ToString();

        // Act
        var refreshToken = _refreshTokenGeneratorService.GenerateToken(userId);

        // Assert
        refreshToken.Should().NotBeNull();
        refreshToken.Id.Should().NotBe(Guid.Empty);
        refreshToken.UserId.Should().Be(userId);
        refreshToken.IsUsed.Should().BeFalse();
        refreshToken.IsRevoked.Should().BeFalse();
        refreshToken.AddedTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        refreshToken.ExpiryTime.Should().BeAfter(DateTime.UtcNow).And.BeBefore(DateTime.UtcNow.AddMonths(2));
    }

    [Fact]
    public async Task ValidateTokenAsync_ShouldThrowException_WhenRefreshTokenNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = Guid.NewGuid().ToString();

        _refreshTokensRepository.Setup(versityRefreshTokensRepository => 
                versityRefreshTokensRepository.FindUserTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))!
            .ReturnsAsync(null as RefreshToken);
        
        // Act
        var act = async () => await _refreshTokenGeneratorService.ValidateTokenAsync(userId, token, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidateTokenAsync_ShouldThrowException_WhenRefreshTokenIsRevoked()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = Guid.NewGuid().ToString();

        _refreshTokensRepository.Setup(versityRefreshTokensRepository =>
            versityRefreshTokensRepository.FindUserTokenAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new RefreshToken() { IsRevoked = true });
        
        // Act
        var act = async () => await _refreshTokenGeneratorService.ValidateTokenAsync(userId, token, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidateTokenAsync_ShouldThrowException_WhenRefreshTokenIsExpired()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = Guid.NewGuid().ToString();

        _refreshTokensRepository.Setup(versityRefreshTokensRepository =>
            versityRefreshTokensRepository.FindUserTokenAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new RefreshToken() { ExpiryTime = DateTime.UtcNow.AddMonths(-1) });
        
        // Act
        var act = async () => await _refreshTokenGeneratorService.ValidateTokenAsync(userId, token, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidateTokenAsync_ShouldReturnValidToken_WhenRefreshTokenIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = Guid.NewGuid().ToString();
        var newToken = Guid.NewGuid().ToString();

        _refreshTokensRepository
            .Setup(versityRefreshTokensRepository => versityRefreshTokensRepository.FindUserTokenAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshToken() 
            { 
                ExpiryTime = DateTime.UtcNow.AddMonths(2), 
                IsRevoked = false, 
                Token = newToken 
            });
        
        // Act
        var result = await _refreshTokenGeneratorService.ValidateTokenAsync(userId, token, CancellationToken.None);
        
        // Assert
        result.Token.Should().Be(newToken);
    }
}