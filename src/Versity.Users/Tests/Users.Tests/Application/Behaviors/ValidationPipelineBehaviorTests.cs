using Application.Behaviors;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Bogus;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Users.Tests.Application.Behaviors;

public class ValidationPipelineBehaviorTests
{
    private readonly IEnumerable<IValidator<ConfirmEmailCommand>> _validators = new List<IValidator<ConfirmEmailCommand>>() 
    { 
        new ConfirmEmailCommandValidation() 
    };
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldThrowException_WhenValidationFails()
    {
        // Arrange
        var pipeline = new ValidationPipelineBehavior<ConfirmEmailCommand, IdentityResult>(_validators);
        
        // Act
        var act = () => pipeline.Handle(
            new ConfirmEmailCommand(new Random().Next(100).ToString(), Guid.NewGuid().ToString()), default!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldInvokeNextPipeline_WhenValidationSucceeds()
    {
        // Arrange
        var mockedPipeline = new Mock<IPipelineBehavior<ConfirmEmailCommand, IdentityResult>>();
        
        // Act
        var act = await GenerateValidationPipelineBehaviorBoilerplate(
            new ConfirmEmailCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()), 
            _validators, 
            () => mockedPipeline.Object.Handle(It.IsAny<ConfirmEmailCommand>(), It.IsAny<RequestHandlerDelegate<IdentityResult>>(), It.IsAny<CancellationToken>()));

        // Assert
        mockedPipeline.Verify(x => x.Handle(It.IsAny<ConfirmEmailCommand>(), It.IsAny<RequestHandlerDelegate<IdentityResult>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldThrowException_WhenValidationFailsInNextPipeline()
    {
        // Arrange
        var validCommand = new ConfirmEmailCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var invalidCommand = new ConfirmEmailCommand(new Random().Next(100).ToString(), Guid.NewGuid().ToString());
        
        // Act
        var act = () => GenerateValidationPipelineBehaviorBoilerplate<ConfirmEmailCommand, IdentityResult>(
            validCommand, 
            _validators, 
            () => GenerateValidationPipelineBehaviorBoilerplate<ConfirmEmailCommand, IdentityResult>(
                invalidCommand, 
                _validators));

        // Assert
        await act.Should().ThrowAsync<ValidationExceptionWithStatusCode>();
    }

    private Task<TResponse> GenerateValidationPipelineBehaviorBoilerplate<TRequest, TResponse>(
        TRequest command,
        IEnumerable<IValidator<TRequest>> validators,
        RequestHandlerDelegate<TResponse> next = default) where TRequest : IRequest<TResponse>
    {
        var pipeline = new ValidationPipelineBehavior<TRequest, TResponse>(validators);
        
        return pipeline.Handle(command, next , CancellationToken.None);
    }
}