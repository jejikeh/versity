using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class RegisterVersityUserTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly RegisterVersityUserCommandHandler _registerVersityUserCommandHandler;
    private readonly RegisterVersityUserCommandValidator _registerVersityUserCommandValidator;

    public RegisterVersityUserTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        
        var emailConfirmMessageService = new Mock<IEmailConfirmMessageService>();
        
        _registerVersityUserCommandValidator = new RegisterVersityUserCommandValidator();
        _registerVersityUserCommandHandler = new RegisterVersityUserCommandHandler(
            _versityUsersRepository.Object, 
            emailConfirmMessageService.Object);
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithAlreadyExists()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
            versityUsersRepository.CreateUserAsync(
                It.IsAny<VersityUser>(),
                It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

        // Act
        var act = async () => await _registerVersityUserCommandHandler.Handle(GenerateFakeRegisterVersityUserCommand(), default);

        // Assert
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenUserDoesNotExists()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
            versityUsersRepository.CreateUserAsync(
                It.IsAny<VersityUser>(),
                It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.SetUserRoleAsync(It.IsAny<VersityUser>(), It.IsAny<VersityRole>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _registerVersityUserCommandHandler.Handle(GenerateFakeRegisterVersityUserCommand(), default);

        // Assert
        result.Succeeded.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordTooShort()
    {
        // Act
        var result = await _registerVersityUserCommandValidator.ValidateAsync(GenerateFakeRegisterVersityUserCommand());
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordTooWeak()
    {
        // Act
        var result = await _registerVersityUserCommandValidator.ValidateAsync(GenerateFakeRegisterVersityUserCommand());
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenPasswordIsStrong()
    {
        // Arrange
        var faker = new Faker();
        var command = new RegisterVersityUserCommand(faker.Name.FirstName(), faker.Name.LastName(), faker.Internet.Email(), "+37533322222", $"{faker.Internet.Password()}!0@aA");
        
        // Act
        var result = await _registerVersityUserCommandValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }

    private static RegisterVersityUserCommand GenerateFakeRegisterVersityUserCommand()
    {
        return new Faker<RegisterVersityUserCommand>().CustomInstantiator(faker => new RegisterVersityUserCommand(
            faker.Name.FirstName(),
            faker.Name.LastName(),
            faker.Internet.Email(),
            faker.Phone.PhoneNumber(),
            faker.Internet.Password()))
            .Generate();
    }
}