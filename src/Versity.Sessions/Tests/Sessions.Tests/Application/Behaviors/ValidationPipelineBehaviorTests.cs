using Application.Behaviors;
using Application.Dtos;
using Application.Exceptions;
using Application.RequestHandlers.Sessions.Commands.CreateSession;
using Bogus;
using Domain.Models;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;

namespace Sessions.Tests.Application.Behaviors;

public class ValidationPipelineBehaviorTests
{
    private readonly IEnumerable<IValidator<CreateSessionCommand>> _validators =
        new List<IValidator<CreateSessionCommand>>()
        {
            new CreateSessionCommandValidator()
        };
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldThrowException_WhenValidationFails()
    {
        // Arrange
        var pipeline = new ValidationPipelineBehavior<CreateSessionCommand, SessionViewModel>(_validators);
        
        // Act
        var act = () => pipeline.Handle(
            GenerateFakeInvalidCreateSessionCommand(), 
            default , 
            CancellationToken.None);

        
        // Assert
        await act.Should().ThrowAsync<ValidationExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldInvokeNextPipeline_WhenValidationSucceeds()
    {
        // Arrange
        var mockedPipeline = new Mock<IPipelineBehavior<CreateSessionCommand, SessionViewModel>>();
        
        // Act
        var act = await GenerateValidationPipelineBehaviorBoilerplate(
            FakeDataGenerator.GenerateFakeCreateSessionCommand(),
            _validators,
            () => mockedPipeline.Object.Handle(
                It.IsAny<CreateSessionCommand>(),
                It.IsAny<RequestHandlerDelegate<SessionViewModel>>(),
                It.IsAny<CancellationToken>()));

        // Assert
        mockedPipeline.Verify(behavior => behavior.Handle(
            It.IsAny<CreateSessionCommand>(), 
            It.IsAny<RequestHandlerDelegate<SessionViewModel>>(), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldThrowException_WhenValidationFailsInNextPipeline()
    {
        // Arrange
        var fakeInvalidCreateProductCommand = GenerateFakeInvalidCreateSessionCommand();
        var fakeInvalidCreateProductCommand1 = GenerateFakeInvalidCreateSessionCommand();
        
        // Act
        var act = () => GenerateValidationPipelineBehaviorBoilerplate(
            fakeInvalidCreateProductCommand, 
            _validators, 
            () => GenerateValidationPipelineBehaviorBoilerplate<CreateSessionCommand, SessionViewModel>(
                fakeInvalidCreateProductCommand1, 
                _validators));

        // Assert
        await act.Should().ThrowAsync<ValidationExceptionWithStatusCode>();
    }

    private static Task<TResponse> GenerateValidationPipelineBehaviorBoilerplate<TRequest, TResponse>(
        TRequest command,
        IEnumerable<IValidator<TRequest>> validators,
        RequestHandlerDelegate<TResponse> next = default) where TRequest : IRequest<TResponse>
    {
        var pipeline = new ValidationPipelineBehavior<TRequest, TResponse>(validators);
        
        return pipeline.Handle(command, next , CancellationToken.None);
    }
    
    private static CreateSessionCommand GenerateFakeInvalidCreateSessionCommand()
    {
        var random = new Random();
        return new Faker<CreateSessionCommand>().CustomInstantiator(faker => new CreateSessionCommand(
            random.Next(1) == 0 ? Guid.NewGuid().ToString() : string.Empty,
            random.Next(1) == 0 ? Guid.NewGuid() : default,
            random.Next(1) == 0 ? faker.Date.Future() : faker.Date.Past(),
            faker.Date.Past()
        )).Generate();
    }
}