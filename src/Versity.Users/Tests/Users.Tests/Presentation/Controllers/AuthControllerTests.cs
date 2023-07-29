using Application.Dtos;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Application.RequestHandlers.Auth.Commands.RefreshJwtToken;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace Users.Tests.Presentation.Controllers;

public class AuthControllerTests
{
    private readonly Mock<ISender> _sender;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _sender = new Mock<ISender>();
        _authController = new AuthController(_sender.Object);
    }
    
    [Fact]
    public async Task Register_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(sender =>
                sender.Send(It.IsAny<RegisterVersityUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var response = await _authController.Register(GenerateFakeRegisterVersityUserCommand(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task ConfirmEmail_ShouldReturnOk_WhenUserExist()
    {
        // Arrange
        _sender.Setup(sender =>
                sender.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var response = await _authController.ConfirmEmail(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task ResendEmailVerificationToken_ShouldReturnOk_WhenUserExist()
    {
        // Arrange
        _sender.Setup(sender =>
                sender.Send(It.IsAny<ResendEmailVerificationTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var response = await _authController.ResendEmailVerificationToken(GenerateFakeResendEmailVerificationTokenCommand(), CancellationToken.None);
        
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenUserExist()
    {
        // Arrange
        _sender.Setup(sender =>
                sender.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthTokens(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
        
        // Act
        var response = await _authController.RefreshToken(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
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
    
    private static ResendEmailVerificationTokenCommand GenerateFakeResendEmailVerificationTokenCommand()
    {
        return new Faker<ResendEmailVerificationTokenCommand>().CustomInstantiator(faker => new ResendEmailVerificationTokenCommand(
                faker.Internet.Email(),
                faker.Internet.Password()))
            .Generate();
    }
}