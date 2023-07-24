using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class LoginVersityUserTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly Mock<IVersityRefreshTokensRepository> _refreshTokensRepository;
    private readonly Mock<IAuthTokenGeneratorService> _authTokenGeneratorService;
    private readonly Mock<IRefreshTokenGeneratorService> _refreshTokenGeneratorService;

    public LoginVersityUserTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _refreshTokensRepository = new Mock<IVersityRefreshTokensRepository>();
        _authTokenGeneratorService = new Mock<IAuthTokenGeneratorService>();
        _refreshTokenGeneratorService = new Mock<IRefreshTokenGeneratorService>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithEmailDoesNotExist()
    {
        // Arrange
        _versityUsersRepository.Setup(
                x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var handler = new LoginVersityUserCommandHandler(
            _authTokenGeneratorService.Object, 
            _versityUsersRepository.Object, 
            _refreshTokensRepository.Object, 
            _refreshTokenGeneratorService.Object);

        // Act
        var act = async () => await handler.Handle(GenerateFakeLoginVersityUserCommand(), default);

        // Assert
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        _versityUsersRepository.Setup(
                x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(
                x => x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        var handler = new LoginVersityUserCommandHandler(
            _authTokenGeneratorService.Object, 
            _versityUsersRepository.Object, 
            _refreshTokensRepository.Object, 
            _refreshTokenGeneratorService.Object);

        // Act
        var act = async () => await handler.Handle(GenerateFakeLoginVersityUserCommand(), default);

        // Assert
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenEmailIsNotConfirmed()
    {
        // Arrange
        _versityUsersRepository.Setup(
                x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(
                x => x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        var handler = new LoginVersityUserCommandHandler(
            _authTokenGeneratorService.Object, 
            _versityUsersRepository.Object, 
            _refreshTokensRepository.Object, 
            _refreshTokenGeneratorService.Object);

        // Act
        var act = async () => await handler.Handle(GenerateFakeLoginVersityUserCommand(), default);

        // Assert
        await act.Should().ThrowAsync<EmailNotConfirmedExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnAuthTokens_WhenEmailIsConfirmedAndPasswordIsCorrectAndUserExists()
    {
        // Arrange
        _versityUsersRepository.Setup(
                x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser { Id = "dd44e461-7217-41ab-8a41-f230381e0ed8", EmailConfirmed = true });
        
        _versityUsersRepository.Setup(
                x => x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _authTokenGeneratorService.Setup(x => 
            x.GenerateToken(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<IEnumerable<string>>()))
            .Returns("dd44e461-7217-41ab-8a41-f230381e0ed8");
        
        _refreshTokenGeneratorService.Setup(x => 
            x.GenerateToken(It.IsAny<string>()))
            .Returns(new RefreshToken() { Token = "dd44e461-7217-41ab-8a41-f230381e0ed8" });
        
        var handler = new LoginVersityUserCommandHandler(
            _authTokenGeneratorService.Object, 
            _versityUsersRepository.Object, 
            _refreshTokensRepository.Object, 
            _refreshTokenGeneratorService.Object);

        // Act
        var result = await handler.Handle(GenerateFakeLoginVersityUserCommand(), default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<AuthTokens>();
        result.Token.Should().NotBeNull();
        result.Token.Should().BeEquivalentTo("dd44e461-7217-41ab-8a41-f230381e0ed8");
        result.RefreshToken.Should().NotBeNull();
        result.Id.Should().BeEquivalentTo("dd44e461-7217-41ab-8a41-f230381e0ed8");
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenEmailIsEmpty()
    {
        // Arrange
        var validator = new LoginVersityUserCommandValidation();
        var command = new LoginVersityUserCommand(string.Empty, "world");
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        // Arrange
        var validator = new LoginVersityUserCommandValidation();
        var command = new LoginVersityUserCommand("email@gmail.com", string.Empty);
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenEmailIsNotValid()
    {
        // Arrange
        var validator = new LoginVersityUserCommandValidation();
        var command = new LoginVersityUserCommand("email@", "world");
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenEmailIsValidAndPasswordIsNotEmpty()
    {
        // Arrange
        var validator = new LoginVersityUserCommandValidation();

        // Act
        var result = await validator.ValidateAsync(GenerateFakeLoginVersityUserCommand());
        
        // Assert
        result.IsValid.Should().BeTrue();
    }

    private LoginVersityUserCommand GenerateFakeLoginVersityUserCommand()
    {
        return new Faker<LoginVersityUserCommand>().CustomInstantiator(faker => new LoginVersityUserCommand(
            faker.Internet.Email(),
            faker.Internet.Password()))
            .Generate();
    }
}