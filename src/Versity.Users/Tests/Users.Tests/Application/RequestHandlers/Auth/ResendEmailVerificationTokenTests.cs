using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class ResendEmailVerificationTokenTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly Mock<IEmailConfirmMessageService> _emailConfirmMessageService;

    public ResendEmailVerificationTokenTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _emailConfirmMessageService = new Mock<IEmailConfirmMessageService>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserWithEmailDoesNotExists()
    {
        _versityUsersRepository.Setup(x =>
                x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var command = new ResendEmailVerificationTokenCommand("hello@gmail.com", "world");
        var handler = new ResendEmailVerificationTokenCommandHandler(
            _versityUsersRepository.Object,
            _emailConfirmMessageService.Object
        );
        
        var act = async () => await handler.Handle(command, default);
        
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        _versityUsersRepository.Setup(x =>
                x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(x =>
                x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        var command = new ResendEmailVerificationTokenCommand("hello@gmail.com", "world");
        var handler = new ResendEmailVerificationTokenCommandHandler(
            _versityUsersRepository.Object,
            _emailConfirmMessageService.Object
        );
        
        var act = async () => await handler.Handle(command, default);
        
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenEmailIsConfirmed()
    {
        _versityUsersRepository.Setup(x =>
                x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser() { EmailConfirmed = true });
        
        _versityUsersRepository.Setup(x =>
                x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        var command = new ResendEmailVerificationTokenCommand("hello@gmail.com", "world");
        var handler = new ResendEmailVerificationTokenCommandHandler(
            _versityUsersRepository.Object,
            _emailConfirmMessageService.Object
        );
        
        var act = async () => await handler.Handle(command, default);
        
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenEmailIsConfirmed()
    {
        _versityUsersRepository.Setup(x =>
                x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(x =>
                x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        var command = new ResendEmailVerificationTokenCommand("hello@gmail.com", "world");
        var handler = new ResendEmailVerificationTokenCommandHandler(
            _versityUsersRepository.Object,
            _emailConfirmMessageService.Object
        );
        
        var result = await handler.Handle(command, default);
        
        result.Succeeded.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenEmailIsEmpty()
    {
        var validator = new ResendEmailVerificationTokenCommandValidator();
        var command = new ResendEmailVerificationTokenCommand(string.Empty, "world");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        var validator = new ResendEmailVerificationTokenCommandValidator();
        var command = new ResendEmailVerificationTokenCommand("email@gmail.com", string.Empty);
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenEmailIsNotValid()
    {
        var validator = new ResendEmailVerificationTokenCommandValidator();
        var command = new ResendEmailVerificationTokenCommand("email@", "world");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenEmailIsValidAndPasswordIsNotEmpty()
    {
        var validator = new ResendEmailVerificationTokenCommandValidator();
        var command = new ResendEmailVerificationTokenCommand("email@gmail.com", "world");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeTrue();
    }
}