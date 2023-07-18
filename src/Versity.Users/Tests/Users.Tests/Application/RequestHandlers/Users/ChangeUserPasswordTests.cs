using System.Security.Claims;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Users.Commands.ChangeUserPassword;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Users;

public class ChangeUserPasswordTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly IEnumerable<Claim> _claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, "dd44e461-7217-41ab-8a41-f230381e0ed8")
    };

    public ChangeUserPasswordTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenClaimsIsEmpty()
    {
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims).Returns(new[]
        {
            new Claim(ClaimTypes.Email, "dd44e461-7217-41ab-8a41-f230381e0ed8")
        });
        
        var request = new ChangeUserPasswordCommand("hello@gmail.com", "world", "123456789");
        var handler = new ChangeUserPasswordCommandHandler(
            _versityUsersRepository.Object,
            _httpContextAccessor.Object
        );
        
        var act = async () => await handler.Handle(request, default);
        
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserIsNotFound()
    {
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims).Returns(_claims);
        
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var request = new ChangeUserPasswordCommand("hello@gmail.com", "world", "123456789");
        var handler = new ChangeUserPasswordCommandHandler(
            _versityUsersRepository.Object,
            _httpContextAccessor.Object
        );
        
        var act = async () => await handler.Handle(request, default);
        
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims).Returns(_claims);
        
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());

        _versityUsersRepository.Setup(x =>
                x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        var request = new ChangeUserPasswordCommand("hello@gmail.com", "world", "123456789");
        var handler = new ChangeUserPasswordCommandHandler(
            _versityUsersRepository.Object,
            _httpContextAccessor.Object
        );
        
        var act = async () => await handler.Handle(request, default);
        
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserIsNotAdminTriesToChangePasswordAnotherUser()
    {
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims).Returns(_claims);
        
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());

        _versityUsersRepository.Setup(x =>
                x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _versityUsersRepository.Setup(x => 
                x.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new List<string>() { "Member" });
        
        var request = new ChangeUserPasswordCommand("hello@gmail.com", "world", "123456789");
        var handler = new ChangeUserPasswordCommandHandler(
            _versityUsersRepository.Object,
            _httpContextAccessor.Object
        );
        
        var act = async () => await handler.Handle(request, default);
        
        await act.Should().ThrowAsync<ExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenAdminTriesToChangePassword()
    {
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims).Returns(_claims);
        
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());

        _versityUsersRepository.Setup(x =>
                x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _versityUsersRepository.Setup(x => 
                x.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });

        _versityUsersRepository.Setup(x =>
                x.ResetPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        var request = new ChangeUserPasswordCommand("hello@gmail.com", "world", "123456789");
        var handler = new ChangeUserPasswordCommandHandler(
            _versityUsersRepository.Object,
            _httpContextAccessor.Object
        );
        
        var result =  await handler.Handle(request, default);
        
        result.Succeeded.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        var validator = new ChangeUserPasswordCommandValidator();
        var command = new ChangeUserPasswordCommand("dd44e461-7217-41ab-8a41-f230381e0ed8", "dd44e461-7217-41ab-8a41-f230381e0ed8", "123456789");
        
        var result = await validator.ValidateAsync(command);
        
        result.IsValid.Should().BeFalse();
    }
}