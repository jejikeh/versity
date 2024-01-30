using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class ResendEmailVerificationTokenTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly ResendEmailVerificationTokenCommandHandler _resendEmailVerificationTokenCommandHandler;
    private readonly ResendEmailVerificationTokenCommandValidator _resendEmailVerificationTokenCommandValidator;
    private readonly Faker _faker = new Faker();

    public ResendEmailVerificationTokenTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        
        var emailConfirmMessageService = new Mock<IEmailConfirmMessageService>();
        
        _resendEmailVerificationTokenCommandValidator = new ResendEmailVerificationTokenCommandValidator();
        _resendEmailVerificationTokenCommandHandler = new ResendEmailVerificationTokenCommandHandler(
            _versityUsersRepository.Object,
            emailConfirmMessageService.Object
        );
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithEmailDoesNotExists()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        // Act
        var act = async () => await _resendEmailVerificationTokenCommandHandler.Handle(GenerateFakeResendEmailVerificationTokenCommand(), default);
        
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
        
        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _resendEmailVerificationTokenCommandHandler.Handle(GenerateFakeResendEmailVerificationTokenCommand(), default);
        
        // Assert
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenEmailIsConfirmed()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser() { EmailConfirmed = true });
        
        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act
        var act = async () => await _resendEmailVerificationTokenCommandHandler.Handle(GenerateFakeResendEmailVerificationTokenCommand(), default);
        
        // Assert
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenEmailIsConfirmed()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act
        var result = await _resendEmailVerificationTokenCommandHandler.Handle(GenerateFakeResendEmailVerificationTokenCommand(), default);
        
        // Assert
        result.Succeeded.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new ResendEmailVerificationTokenCommand(string.Empty, _faker.Internet.Password() + "$#E0d");
        
        // Act
        var result = await _resendEmailVerificationTokenCommandValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        // Arrange
        var command = new ResendEmailVerificationTokenCommand(_faker.Internet.Email(), string.Empty);
        
        // Act
        var result = await _resendEmailVerificationTokenCommandValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenEmailIsNotValid()
    {
        // Arrange
        var command = new ResendEmailVerificationTokenCommand(_faker.Lorem.Word(), Guid.NewGuid().ToString());
        
        // Act
        var result = await _resendEmailVerificationTokenCommandValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenEmailIsValidAndPasswordIsNotEmpty()
    {
        // Arrange
        var command = new ResendEmailVerificationTokenCommand(_faker.Internet.Email(), Guid.NewGuid().ToString());
        
        // Act
        var result = await _resendEmailVerificationTokenCommandValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }

    private static ResendEmailVerificationTokenCommand GenerateFakeResendEmailVerificationTokenCommand()
    {
        return new Faker<ResendEmailVerificationTokenCommand>().CustomInstantiator(faker => new ResendEmailVerificationTokenCommand(
            faker.Internet.Email(),
            faker.Internet.Password()))
            .Generate();
    }
}