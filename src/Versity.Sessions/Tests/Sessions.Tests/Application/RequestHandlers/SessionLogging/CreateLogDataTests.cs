using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogData;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.SessionLogging;

public class CreateLogDataTests
{
    private readonly Mock<ILogsDataRepository> _logsDataRepository;
    private readonly Mock<ISessionLogsRepository> _sessionsRepository;
    private readonly CreateLogDataCommandHandler _createLogDataCommandHandler;

    public CreateLogDataTests()
    {
        _logsDataRepository = new Mock<ILogsDataRepository>();
        _sessionsRepository = new Mock<ISessionLogsRepository>();
        _createLogDataCommandHandler = new CreateLogDataCommandHandler(_logsDataRepository.Object, _sessionsRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSessionDoesNotExist()
    {
        // Arrange
        _sessionsRepository.Setup(repository => 
                repository.GetSessionLogsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as SessionLogs);
        
        // Act
        var act = async () => await _createLogDataCommandHandler.Handle(
            FakeDataGenerator.GenerateFakeCreateLogDataCommand(), 
            CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldAddLogToSessionLogs_WhenSessionExist()
    {
        // Arrange
        _sessionsRepository.Setup(repository => 
                repository.GetSessionLogsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionLogs(new Random().Next(1,20)));
        
        _logsDataRepository.Setup(repository =>
                repository.CreateLogDataAsync(It.IsAny<LogData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeLogData);
        
        var command = FakeDataGenerator.GenerateFakeCreateLogDataCommand();
        
        // Act
        var result = await _createLogDataCommandHandler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
    }
}