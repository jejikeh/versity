using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Application.RequestHandlers.Auth.Commands.RefreshJwtToken;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class RefreshJwtTokenTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly Mock<IAuthTokenGeneratorService> _authTokenGeneratorService;
    private readonly Mock<IRefreshTokenGeneratorService> _refreshTokenGeneratorService;
    private readonly RefreshTokenCommandHandler _refreshTokenCommandHandler;

    public RefreshJwtTokenTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _authTokenGeneratorService = new Mock<IAuthTokenGeneratorService>();
        _refreshTokenGeneratorService = new Mock<IRefreshTokenGeneratorService>();
        
        var refreshTokensRepository = new Mock<IVersityRefreshTokensRepository>();
        
        _refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            refreshTokensRepository.Object, 
            _versityUsersRepository.Object, 
            _authTokenGeneratorService.Object, 
            _refreshTokenGeneratorService.Object);
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);

        _refreshTokenGeneratorService.Setup(refreshTokenGeneratorService => 
            refreshTokenGeneratorService.ValidateTokenAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync(new RefreshToken());
        
        var command = new RefreshTokenCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        
        // Act
        var act = async () => await _refreshTokenCommandHandler.Handle(command, default);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnAuthToken_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = Guid.NewGuid().ToString();
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser() { Id = userId });
        
        _refreshTokenGeneratorService.Setup(refreshTokenGeneratorService => 
                refreshTokenGeneratorService.ValidateTokenAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshToken() { Token = userId });

        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new List<string>() { "Member" });
        
        _authTokenGeneratorService.Setup(authTokenGeneratorService => 
                authTokenGeneratorService.GenerateToken(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<IEnumerable<string>>()))
            .Returns(token);

        var command = new RefreshTokenCommand(userId, token);

        // Act
        var result = await _refreshTokenCommandHandler.Handle(command, default);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<AuthTokens>();
        result.Token.Should().NotBeNull();
        result.Token.Should().BeEquivalentTo(token);
        result.RefreshToken.Should().NotBeNull();
        result.RefreshToken.Should().BeEquivalentTo(userId);
        result.Id.Should().BeEquivalentTo(userId);
    }
}