using Application.Behaviors;
using Application.Dtos;
using Application.RequestHandlers.Sessions.Commands.CreateSession;
using Bogus;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Sessions.Tests.Application.Behaviors;

public class LoggingPipelineBehaviorTests
{
    private readonly Mock<ILogger<LoggingPipelineBehavior<CreateSessionCommand, SessionViewModel>>> _loggerMock;

    public LoggingPipelineBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingPipelineBehavior<CreateSessionCommand, SessionViewModel>>>();
    }
    
    [Fact]
    public async Task LoggingPipelineBehavior_ShouldInvokeNextPipeline_WhenDelegateIsValid()
    {
        // Arrange
        var mockedPipeline = new Mock<IPipelineBehavior<CreateSessionCommand, SessionViewModel>>();
        
        // Act
        var act = await GenerateLoggingPipelineBehaviorBoilerplate(
            FakeDataGenerator.GenerateFakeCreateSessionCommand(), 
            () => mockedPipeline.Object.Handle(It.IsAny<CreateSessionCommand>(), It.IsAny<RequestHandlerDelegate<SessionViewModel>>(), It.IsAny<CancellationToken>()));

        // Assert
        mockedPipeline.Verify(pipelineBehavior => 
            pipelineBehavior.Handle(
                It.IsAny<CreateSessionCommand>(), 
                It.IsAny<RequestHandlerDelegate<SessionViewModel>>(), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    private Task<SessionViewModel> GenerateLoggingPipelineBehaviorBoilerplate(
        CreateSessionCommand command,
        RequestHandlerDelegate<SessionViewModel> next = default)
    {
        var pipeline = new LoggingPipelineBehavior<CreateSessionCommand, SessionViewModel>(_loggerMock.Object);
        
        return pipeline.Handle(command, next , CancellationToken.None);
    }
}