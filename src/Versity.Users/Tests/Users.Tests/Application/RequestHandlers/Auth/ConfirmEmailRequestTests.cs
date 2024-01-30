using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Utils = Application.Common.Utils;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class ConfirmEmailRequestTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly ConfirmEmailCommandHandler _confirmEmailCommandHandler;
    private readonly ConfirmEmailCommandValidation _confirmEmailCommandValidation;

    public ConfirmEmailRequestTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _confirmEmailCommandHandler = new ConfirmEmailCommandHandler(_versityUsersRepository.Object);
        _confirmEmailCommandValidation = new ConfirmEmailCommandValidation();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        // Act
        var act = async () => await _confirmEmailCommandHandler.Handle(GenerateFakeConfirmEmailCommand(), default);

        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenTokenIsInvalidAndUserExists()
    {
        // Arrange
        _versityUsersRepository.Setup(repository =>  
                repository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(repository => 
                repository.ConfirmUserEmail(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        // Act
        var act = async () => await _confirmEmailCommandHandler.Handle(GenerateFakeConfirmEmailCommand(), default);

        // Assert
        await act.Should().ThrowAsync<IdentityExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnSuccessIdentityResult_WhenTokenIsValidAndUserExists()
    {
        // Arrange
        _versityUsersRepository.Setup(usersRepository => 
                usersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        _versityUsersRepository.Setup(usersRepository => 
                usersRepository.ConfirmUserEmail(It.IsAny<VersityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _confirmEmailCommandHandler.Handle(GenerateFakeConfirmEmailCommand(), default);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNull()
    {
        // Arrange
        var command = new ConfirmEmailCommand(string.Empty, Guid.NewGuid().ToString());
        
        // Act
        var result = await _confirmEmailCommandValidation.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenIdIsNotGuid()
    {
        // Arrange
        var command = new ConfirmEmailCommand(Utils.GenerateRandomString(7), Guid.NewGuid().ToString());
        
        // Act
        var result = await _confirmEmailCommandValidation.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenTokenIsEmpty()
    {
        // Arrange
        var command = new ConfirmEmailCommand(Guid.NewGuid().ToString(), string.Empty);
        
        // Act
        var result = await _confirmEmailCommandValidation.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenTokenIsValidAndUserIsGuid()
    {
        // Arrange
        var command = GenerateFakeConfirmEmailCommand();
        
        // Act
        var result = await _confirmEmailCommandValidation.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }

    private static ConfirmEmailCommand GenerateFakeConfirmEmailCommand()
    {
        return new Faker<ConfirmEmailCommand>().CustomInstantiator(faker => new ConfirmEmailCommand(
                faker.Random.Guid().ToString(),
                faker.Random.Guid().ToString()))
            .Generate();
    }
}