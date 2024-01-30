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
    private readonly Mock<IAuthTokenGeneratorService> _authTokenGeneratorService;
    private readonly Mock<IRefreshTokenGeneratorService> _refreshTokenGeneratorService;
    private readonly LoginVersityUserCommandHandler _loginVersityUserCommandHandler;
    private readonly LoginVersityUserCommandValidation _loginVersityUserCommandValidation;

    public LoginVersityUserTests()
    {
        var refreshTokensRepository = new Mock<IVersityRefreshTokensRepository>();
        
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _authTokenGeneratorService = new Mock<IAuthTokenGeneratorService>();
        _refreshTokenGeneratorService = new Mock<IRefreshTokenGeneratorService>();
        
        
        _loginVersityUserCommandHandler = new LoginVersityUserCommandHandler(
            _authTokenGeneratorService.Object, 
            _versityUsersRepository.Object, 
            refreshTokensRepository.Object, 
            _refreshTokenGeneratorService.Object);
        
        _loginVersityUserCommandValidation = new LoginVersityUserCommandValidation();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithEmailDoesNotExist()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        // Act
        var act = async () => await _loginVersityUserCommandHandler.Handle(GenerateFakeLoginVersityUserCommand(), default);

        // Assert
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(
                x => x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        // Act
        var act = async () => await _loginVersityUserCommandHandler.Handle(GenerateFakeLoginVersityUserCommand(), default);

        // Assert
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenEmailIsNotConfirmed()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act
        var act = async () => await _loginVersityUserCommandHandler.Handle(GenerateFakeLoginVersityUserCommand(), default);

        // Assert
        await act.Should().ThrowAsync<EmailNotConfirmedExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnAuthTokens_WhenEmailIsConfirmedAndPasswordIsCorrectAndUserExists()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser { Id = id, EmailConfirmed = true });
        
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _authTokenGeneratorService.Setup(authTokenGeneratorService => 
            authTokenGeneratorService.GenerateToken(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<IEnumerable<string>>()))
            .Returns(id);
        
        _refreshTokenGeneratorService.Setup(refreshTokenGeneratorService => 
            refreshTokenGeneratorService.GenerateToken(It.IsAny<string>()))
            .Returns(new RefreshToken() { Token = id });
        
        // Act
        var result = await _loginVersityUserCommandHandler.Handle(GenerateFakeLoginVersityUserCommand(), default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<AuthTokens>();
        result.Token.Should().NotBeNull();
        result.Token.Should().BeEquivalentTo(id);
        result.RefreshToken.Should().NotBeNull();
        result.Id.Should().BeEquivalentTo(id);
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new LoginVersityUserCommand(string.Empty, new Faker().Lorem.Word());
        
        // Act
        var result = await _loginVersityUserCommandValidation.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        // Arrange
        var command = new LoginVersityUserCommand(new Faker().Internet.Email(), string.Empty);
        
        // Act
        var result = await _loginVersityUserCommandValidation.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenEmailIsNotValid()
    {
        // Arrange
        var faker = new Faker();
        var command = new LoginVersityUserCommand(faker.Lorem.Word(), faker.Lorem.Word());
        
        // Act
        var result = await _loginVersityUserCommandValidation.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenEmailIsValidAndPasswordIsNotEmpty()
    {
        // Act
        var result = await _loginVersityUserCommandValidation.ValidateAsync(GenerateFakeLoginVersityUserCommand());
        
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