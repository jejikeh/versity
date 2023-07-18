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
        _versityUsersRepository.Setup(
            x => x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var command = new ConfirmEmailCommand("_", "_");
        var handler = new ConfirmEmailCommandHandler(_versityUsersRepository.Object);

        var act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenTokenIsInvalidAndUserExists()
    {
        _versityUsersRepository.Setup(
                x => x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(
                x => x.ConfirmUserEmail(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        var command = new ConfirmEmailCommand("_", "_");
        var handler = new ConfirmEmailCommandHandler(_versityUsersRepository.Object);

        var act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenTokenIsValidAndUserExists()
    {
        _versityUsersRepository.Setup(
                x => x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(
                x => x.ConfirmUserEmail(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        var command = new ConfirmEmailCommand("dd44e461-7217-41ab-8a41-f230381e0ed8", "dd44e461-7217-41ab-8a41-f230381e0ed8");
        var handler = new ConfirmEmailCommandHandler(_versityUsersRepository.Object);

        var result = await handler.Handle(command, default);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNull()
    {
        var validator = new ConfirmEmailCommandValidation();
        var command = new ConfirmEmailCommand(null, "dd44e461-7217-41ab-8a41-f230381e0ed8");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNotGuid()
    {
        var validator = new ConfirmEmailCommandValidation();
        var command = new ConfirmEmailCommand("dd44e461-7217-41ab-8a41-f230381ed8", "dd44e461-7217-41ab-8a41-f230381e0ed8");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenTokenIsEmpty()
    {
        var validator = new ConfirmEmailCommandValidation();
        var command = new ConfirmEmailCommand("dd44e461-7217-41ab-8a41-f230381e0ed8", string.Empty);
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenTokenIsValidAndUserIsGuid()
    {
        var validator = new ConfirmEmailCommandValidation();
        var command = new ConfirmEmailCommand("dd44e461-7217-41ab-8a41-f230381e0ed8", "dd44e461-7217-41ab-8a41-f230381e0ed8");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeTrue();
    }
}