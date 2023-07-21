using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Services.TokenServices;
using Moq;

namespace Users.Tests.Infrastructure.TokenServices;

public class RefreshTokenGeneratorServiceTests
{
    private readonly Mock<IVersityRefreshTokensRepository> _refreshTokensRepository;

    public RefreshTokenGeneratorServiceTests()
    {
        _refreshTokensRepository = new Mock<IVersityRefreshTokensRepository>();
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken_WhenCalled()
    {
        var userId = "testUserId";
        var tokenService = new RefreshTokenGeneratorService(_refreshTokensRepository.Object);

        var refreshToken = tokenService.GenerateToken(userId);

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
        var userId = "testUserId";
        var token = "testToken";
        var tokenService = new RefreshTokenGeneratorService(_refreshTokensRepository.Object);

        _refreshTokensRepository.Setup(x =>
                x.FindUserTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))!
            .ReturnsAsync(null as RefreshToken);
        
        var act = async () => await tokenService.ValidateTokenAsync(userId, token, CancellationToken.None);
        
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidateTokenAsync_ShouldThrowException_WhenRefreshTokenIsRevoked()
    {
        var userId = "testUserId";
        var token = "testToken";
        var tokenService = new RefreshTokenGeneratorService(_refreshTokensRepository.Object);

        _refreshTokensRepository.Setup(x =>
            x.FindUserTokenAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new RefreshToken() { IsRevoked = true });
        
        var act = async () => await tokenService.ValidateTokenAsync(userId, token, CancellationToken.None);
        
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidateTokenAsync_ShouldThrowException_WhenRefreshTokenIsExpired()
    {
        var userId = "testUserId";
        var token = "testToken";
        var tokenService = new RefreshTokenGeneratorService(_refreshTokensRepository.Object);

        _refreshTokensRepository.Setup(x =>
            x.FindUserTokenAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new RefreshToken() { ExpiryTime = DateTime.UtcNow.AddMonths(-1) });
        
        var act = async () => await tokenService.ValidateTokenAsync(userId, token, CancellationToken.None);
        
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidateTokenAsync_ShouldReturnValidToken_WhenRefreshTokenIsValid()
    {
        var userId = "testUserId";
        var token = "testToken";
        var tokenService = new RefreshTokenGeneratorService(_refreshTokensRepository.Object);

        _refreshTokensRepository.Setup(x =>
            x.FindUserTokenAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new RefreshToken()
        {
            ExpiryTime = DateTime.UtcNow.AddMonths(2), 
            IsRevoked = false,
            Token = "testToken"
        });
        
        var result = await tokenService.ValidateTokenAsync(userId, token, CancellationToken.None);
        
        result.Token.Should().Be("testToken");
    }
}