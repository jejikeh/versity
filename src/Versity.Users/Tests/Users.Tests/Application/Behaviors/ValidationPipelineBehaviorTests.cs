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
    private readonly IEnumerable<IValidator<ConfirmEmailCommand>> _validators =
        new List<IValidator<ConfirmEmailCommand>>()
        {
            new ConfirmEmailCommandValidation()
        };
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldThrowException_WhenValidationFails()
    {
        var pipeline = new ValidationPipelineBehavior<ConfirmEmailCommand, IdentityResult>(_validators);
        
        var act = () => pipeline.Handle(new ConfirmEmailCommand("1", "2"), default , CancellationToken.None);

        await act.Should().ThrowAsync<ValidationExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldInvokeNextPipeline_WhenValidationSucceeds()
    {
        var mockedPipeline = new Mock<IPipelineBehavior<ConfirmEmailCommand, IdentityResult>>();
        
        var act = await GenerateValidationPipelineBehaviorBoilerplate(
            new ConfirmEmailCommand("6163bc73-5c9f-4df0-8db6-feba832d5d47", "2"), 
            _validators, 
            () => mockedPipeline.Object.Handle(It.IsAny<ConfirmEmailCommand>(), It.IsAny<RequestHandlerDelegate<IdentityResult>>(), It.IsAny<CancellationToken>()));

        mockedPipeline.Verify(x => x.Handle(It.IsAny<ConfirmEmailCommand>(), It.IsAny<RequestHandlerDelegate<IdentityResult>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldThrowException_WhenValidationFailsInNextPipeline()
    {
        var act = () => GenerateValidationPipelineBehaviorBoilerplate<ConfirmEmailCommand, IdentityResult>(
            new ConfirmEmailCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()), 
            _validators, 
            () => GenerateValidationPipelineBehaviorBoilerplate<ConfirmEmailCommand, IdentityResult>(
                new ConfirmEmailCommand("1", "2"), 
                _validators));

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