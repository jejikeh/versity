using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
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
        _versityUsersRepository.Setup(x =>
            x.CreateUserAsync(
                It.IsAny<VersityUser>(),
                It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());
        
        var command = new RegisterVersityUserCommand("John", "Doe", "hello@gmail.com", "+123456789", "world");
        var handler = new RegisterVersityUserCommandHandler(
            _versityUsersRepository.Object, 
            _emailConfirmMessageService.Object);

        var act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenUserDoesNotExists()
    {
        _versityUsersRepository.Setup(x =>
            x.CreateUserAsync(
                It.IsAny<VersityUser>(),
                It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        _versityUsersRepository.Setup(
            x => x.SetUserRoleAsync(It.IsAny<VersityUser>(), It.IsAny<VersityRole>()))
            .ReturnsAsync(IdentityResult.Success);
        
        var command = new RegisterVersityUserCommand("John", "Doe", "hello@gmail.com", "+123456789", "world");
        var handler = new RegisterVersityUserCommandHandler(
            _versityUsersRepository.Object, 
            _emailConfirmMessageService.Object);

        var result = await handler.Handle(command, default);

        result.Succeeded.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordTooShort()
    {
        var validator = new RegisterVersityUserCommandValidator();
        var command = new RegisterVersityUserCommand("John", "Doe", "hello@gmail.com", "+123456789", "world");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordTooWeak()
    {
        var validator = new RegisterVersityUserCommandValidator();
        var command = new RegisterVersityUserCommand("John", "Doe", "hello@gmail.com", "+123456789", "worldworld");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenPasswordIsStrong()
    {
        var validator = new RegisterVersityUserCommandValidator();
        var command = new RegisterVersityUserCommand("John", "Doe", "hello@gmail.com", "+37533322222", "worldWorld123!");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeTrue();
    }
}