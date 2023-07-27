using System.Security.Claims;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Users.Commands.ChangeUserPassword;
using Bogus;
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
    private readonly ChangeUserPasswordCommandHandler _changeUserPasswordCommandHandler;
    private readonly ChangeUserPasswordCommandValidator _changeUserPasswordCommandValidator;

    public ChangeUserPasswordTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _httpContextAccessor.Setup(httpContextAccessor => httpContextAccessor.HttpContext.User.Claims).Returns(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        });
        
        _changeUserPasswordCommandHandler = new ChangeUserPasswordCommandHandler(_versityUsersRepository.Object, _httpContextAccessor.Object);
        _changeUserPasswordCommandValidator = new ChangeUserPasswordCommandValidator();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenClaimsIsEmpty()
    {
        // Arrange
        _httpContextAccessor.Setup(httpContextAccessor => httpContextAccessor.HttpContext.User.Claims).Returns(new[]
        {
            new Claim(ClaimTypes.Email, new Faker().Internet.Email())
        });
        
        // Act
        var act = async () => await _changeUserPasswordCommandHandler.Handle(GenerateFakeChangeUserPasswordCommand(), default);
        
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserIsNotFound()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        // Act
        var act = async () => await _changeUserPasswordCommandHandler.Handle(GenerateFakeChangeUserPasswordCommand(), default);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());

        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        // Act
        var act = async () => await _changeUserPasswordCommandHandler.Handle(GenerateFakeChangeUserPasswordCommand(), default);
        
        // Assert
        await act.Should().ThrowAsync<IncorrectEmailOrPasswordExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserIsNotAdminTriesToChangePasswordAnotherUser()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());

        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new List<string>() { "Member" });
        
        // Act
        var act = async () => await _changeUserPasswordCommandHandler.Handle(GenerateFakeChangeUserPasswordCommand(), default);
        
        // Assert
        await act.Should().ThrowAsync<ExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenAdminTriesToChangePassword()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());

        _versityUsersRepository.Setup(x =>
                x.CheckPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });

        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.ResetPasswordAsync(It.IsAny<VersityUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _changeUserPasswordCommandHandler.Handle(GenerateFakeChangeUserPasswordCommand(), default);
        
        // Assert
        result.Succeeded.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        // Arrange
        var faker = new Faker();
        var command = new ChangeUserPasswordCommand(
            faker.Internet.Password(), 
            faker.Internet.Password(), 
            Guid.NewGuid().ToString());
        
        // Act
        var result = await _changeUserPasswordCommandValidator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }

    private static ChangeUserPasswordCommand GenerateFakeChangeUserPasswordCommand()
    {
        return new Faker<ChangeUserPasswordCommand>().CustomInstantiator(faker => new ChangeUserPasswordCommand(
            faker.Internet.Password(),
            faker.Internet.Password(),
            Guid.NewGuid().ToString())).Generate();
    }
}