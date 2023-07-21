using Application.Dtos;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Application.RequestHandlers.Auth.Commands.RefreshJwtToken;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace Users.Tests.Presentation.Controllers;

public class AuthControllerTests
{
    private Mock<ISender> _sender;

    public AuthControllerTests()
    {
        _sender = new Mock<ISender>();
    }
    
    [Fact]
    public async Task Register_ShouldReturnOk_WhenCommandIsValid()
    {
        _sender.Setup(x =>
                x.Send(It.IsAny<RegisterVersityUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        var request = new RegisterVersityUserCommand("test", "test", "test", "test", "test");
        var service = new AuthController(_sender.Object);
        
        var response = await service.Register(request, CancellationToken.None);
        
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task ConfirmEmail_ShouldReturnOk_WhenUserExist()
    {
        _sender.Setup(x =>
                x.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        var service = new AuthController(_sender.Object);
        
        var response = await service.ConfirmEmail("test", "test", CancellationToken.None);
        
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task ResendEmailVerificationToken_ShouldReturnOk_WhenUserExist()
    {
        _sender.Setup(x =>
                x.Send(It.IsAny<ResendEmailVerificationTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        var request = new ResendEmailVerificationTokenCommand("test", "test");
        var service = new AuthController(_sender.Object);
        
        var response = await service.ResendEmailVerificationToken(request, CancellationToken.None);
        
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenUserExist()
    {
        _sender.Setup(x =>
                x.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthTokens("test", "test", "test"));
        var service = new AuthController(_sender.Object);
        
        var response = await service.RefreshToken("test", "test", CancellationToken.None);
        
        response.Should().BeOfType<OkObjectResult>();
    }
}