using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogsData;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.SessionLogging;

public class CreateLogsDataTests
{
    private readonly Mock<ILogsDataRepository> _logsDataRepository;
    private readonly Mock<ISessionLogsRepository> _sessionsRepository;

    public CreateLogsDataTests()
    {
        _logsDataRepository = new Mock<ILogsDataRepository>();
        _sessionsRepository = new Mock<ISessionLogsRepository>();
    }
    
    [Fact]
    public async Task Handle_ShouldThrowException_WhenSessionDoesNotExist()
    {
        // Arrange
        _sessionsRepository.Setup(repository => 
                repository.GetSessionLogsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as SessionLogs);
        
        var handler = new CreateLogsDataCommandHandler(_logsDataRepository.Object, _sessionsRepository.Object);
        
        // Act
        var act = async () => await handler.Handle(
            FakeDataGenerator.GenerateFakeCreateLogsDataCommand(20), 
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
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionLogs(20));

        var handler = new CreateLogsDataCommandHandler(_logsDataRepository.Object, _sessionsRepository.Object);
        var command = FakeDataGenerator.GenerateFakeCreateLogsDataCommand(20);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.ToList().Count.Should().Be(20);
    }
}