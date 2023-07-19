using Application.Behaviors;
using Application.Exceptions;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Users.Tests.Application.Behaviors;

public class LoggingPipelineBehaviorTests
{
    private readonly Mock<ILogger<LoggingPipelineBehavior<ConfirmEmailCommand, IdentityResult>>> _loggerMock;

    public LoggingPipelineBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingPipelineBehavior<ConfirmEmailCommand, IdentityResult>>>();
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldInvokeNextPipeline_WhenDelegateIsValid()
    {
        var mockedPipeline = new Mock<IPipelineBehavior<ConfirmEmailCommand, IdentityResult>>();
        
        var act = await GenerateLoggingPipelineBehaviorBoilerplate(
            new ConfirmEmailCommand("6163bc73-5c9f-4df0-8db6-feba832d5d47", "2"), 
            () => mockedPipeline.Object.Handle(It.IsAny<ConfirmEmailCommand>(), It.IsAny<RequestHandlerDelegate<IdentityResult>>(), It.IsAny<CancellationToken>()));

        mockedPipeline.Verify(x => x.Handle(It.IsAny<ConfirmEmailCommand>(), It.IsAny<RequestHandlerDelegate<IdentityResult>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    private Task<IdentityResult> GenerateLoggingPipelineBehaviorBoilerplate(
        ConfirmEmailCommand command,
        RequestHandlerDelegate<IdentityResult> next = default)
    {
        var pipeline = new LoggingPipelineBehavior<ConfirmEmailCommand, IdentityResult>(_loggerMock.Object);
        
        return pipeline.Handle(command, next , CancellationToken.None);
    }
}