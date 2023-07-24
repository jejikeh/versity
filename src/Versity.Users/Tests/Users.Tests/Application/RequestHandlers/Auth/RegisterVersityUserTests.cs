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
    private readonly Mock<IEmailConfirmMessageService> _emailConfirmMessageService;

    public RegisterVersityUserTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _emailConfirmMessageService = new Mock<IEmailConfirmMessageService>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithAlreadyExists()
    {
        // Arrange
        _versityUsersRepository.Setup(x =>
            x.CreateUserAsync(
                It.IsAny<VersityUser>(),
                It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());
        
        var handler = new RegisterVersityUserCommandHandler(
            _versityUsersRepository.Object, 
            _emailConfirmMessageService.Object);

        // Act
        var act = async () => await handler.Handle(GenerateFakeRegisterVersityUserCommand(), default);

        // Assert
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenUserDoesNotExists()
    {
        // Arrange
        _versityUsersRepository.Setup(x =>
            x.CreateUserAsync(
                It.IsAny<VersityUser>(),
                It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        _versityUsersRepository.Setup(
            x => x.SetUserRoleAsync(It.IsAny<VersityUser>(), It.IsAny<VersityRole>()))
            .ReturnsAsync(IdentityResult.Success);
        
        var handler = new RegisterVersityUserCommandHandler(
            _versityUsersRepository.Object, 
            _emailConfirmMessageService.Object);

        // Act
        var result = await handler.Handle(GenerateFakeRegisterVersityUserCommand(), default);

        // Assert
        result.Succeeded.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordTooShort()
    {
        // Arrange
        var validator = new RegisterVersityUserCommandValidator();
        
        // Act
        var result = await validator.ValidateAsync(GenerateFakeRegisterVersityUserCommand());
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordTooWeak()
    {
        // Arrange
        var validator = new RegisterVersityUserCommandValidator();
        
        // Act
        var result = await validator.ValidateAsync(GenerateFakeRegisterVersityUserCommand());
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenPasswordIsStrong()
    {
        // Arrange
        var validator = new RegisterVersityUserCommandValidator();
        var command = new RegisterVersityUserCommand("John", "Doe", "hello@gmail.com", "+37533322222", "worldWorld123!");
        
        // Act
        var result = await validator.ValidateAsync(command);
        
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