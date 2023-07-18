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
    private readonly Mock<IVersityRefreshTokensRepository> _refreshTokensRepository;
    private readonly Mock<IAuthTokenGeneratorService> _authTokenGeneratorService;
    private readonly Mock<IRefreshTokenGeneratorService> _refreshTokenGeneratorService;

    public RefreshJwtTokenTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _refreshTokensRepository = new Mock<IVersityRefreshTokensRepository>();
        _authTokenGeneratorService = new Mock<IAuthTokenGeneratorService>();
        _refreshTokenGeneratorService = new Mock<IRefreshTokenGeneratorService>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExist()
    {
        _versityUsersRepository.Setup(
                x => x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);

        _refreshTokenGeneratorService.Setup(
            x => x.ValidateTokenAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync(new RefreshToken());
        
        var command = new RefreshTokenCommand("1234-12524-12415-125", "1234-12524-12415-125");
        var handler = new RefreshTokenCommandHandler(
            _refreshTokensRepository.Object, 
            _versityUsersRepository.Object, 
            _authTokenGeneratorService.Object, 
            _refreshTokenGeneratorService.Object);
        
        var act = async () => await handler.Handle(command, default);
        
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnAuthToken_WhenUserExists()
    {
        _versityUsersRepository.Setup(
                x => x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser() { Id = "dd44e461-7217-41ab-8a41-f230381e0ed8" });
        
        _refreshTokenGeneratorService.Setup(
            x => x.ValidateTokenAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()))!.ReturnsAsync(new RefreshToken() { Token = "dd44e461-7217-41ab-8a41-f230381e0ed8"});

        _versityUsersRepository.Setup(
                x => x.GetRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new List<string>() { "Member" });
        
        _authTokenGeneratorService.Setup(
            x => x.GenerateToken(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<string>>()))
            .Returns("1234-12524-12415-125");

        var command = new RefreshTokenCommand("dd44e461-7217-41ab-8a41-f230381e0ed8", "1234-12524-12415-125");
        var handler = new RefreshTokenCommandHandler(
            _refreshTokensRepository.Object, 
            _versityUsersRepository.Object, 
            _authTokenGeneratorService.Object, 
            _refreshTokenGeneratorService.Object);
        
        var result = await handler.Handle(command, default);
        
        result.Should().NotBeNull();
        result.Should().BeOfType<AuthTokens>();
        result.Token.Should().NotBeNull();
        result.Token.Should().BeEquivalentTo("1234-12524-12415-125");
        result.RefreshToken.Should().NotBeNull();
        result.RefreshToken.Should().BeEquivalentTo("dd44e461-7217-41ab-8a41-f230381e0ed8");
        result.Id.Should().BeEquivalentTo("dd44e461-7217-41ab-8a41-f230381e0ed8");
    }
}