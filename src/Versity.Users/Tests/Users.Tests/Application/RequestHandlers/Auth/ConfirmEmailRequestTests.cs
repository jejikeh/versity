using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class ConfirmEmailRequestTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;

    public ConfirmEmailRequestTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        _versityUsersRepository.Setup(
            x => x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var command = new ConfirmEmailCommand("_", "_");
        var handler = new ConfirmEmailCommandHandler(_versityUsersRepository.Object);

        // Act
        var act = async () => await handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenTokenIsInvalidAndUserExists()
    {
        // Arrange
        _versityUsersRepository.Setup(
                repository =>  repository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(
                repository => repository.ConfirmUserEmail(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        var command = new ConfirmEmailCommand("_", "_");
        var handler = new ConfirmEmailCommandHandler(_versityUsersRepository.Object);

        // Act
        var act = async () => await handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenTokenIsValidAndUserExists()
    {
        // Arrange
        _versityUsersRepository.Setup(
                usersRepository => usersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(
                usersRepository => usersRepository.ConfirmUserEmail(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        var command = new ConfirmEmailCommand("dd44e461-7217-41ab-8a41-f230381e0ed8", "dd44e461-7217-41ab-8a41-f230381e0ed8");
        var handler = new ConfirmEmailCommandHandler(_versityUsersRepository.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNull()
    {
        // Arrange
        var validator = new ConfirmEmailCommandValidation();
        var command = new ConfirmEmailCommand(null, "dd44e461-7217-41ab-8a41-f230381e0ed8");
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNotGuid()
    {
        // Arrange
        var validator = new ConfirmEmailCommandValidation();
        var command = new ConfirmEmailCommand("dd44e461-7217-41ab-8a41-f230381ed8", "dd44e461-7217-41ab-8a41-f230381e0ed8");
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenTokenIsEmpty()
    {
        // Arrange
        var validator = new ConfirmEmailCommandValidation();
        var command = new ConfirmEmailCommand("dd44e461-7217-41ab-8a41-f230381e0ed8", string.Empty);
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenTokenIsValidAndUserIsGuid()
    {
        // Arrange
        var validator = new ConfirmEmailCommandValidation();
        var command = new ConfirmEmailCommand("dd44e461-7217-41ab-8a41-f230381e0ed8", "dd44e461-7217-41ab-8a41-f230381e0ed8");
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
}