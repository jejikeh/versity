using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.SessionLogging.Queries.GetSessionLogsById;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.SessionLogging;

public class GetSessionLogsByIdTests
{
    private readonly Mock<ISessionLogsRepository> _sessionLogsRepository;

    public GetSessionLogsByIdTests()
    {
        _sessionLogsRepository = new Mock<ISessionLogsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenSessionLogsIsNotFound()
    {
        // Arrange
        var handler = new GetSessionLogsByIdQueryHandler(_sessionLogsRepository.Object);
        _sessionLogsRepository.Setup(x =>
                x.GetSessionLogsByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as SessionLogs);
        
        // Act
        var act = async () => await handler.Handle(
            new GetSessionLogsByIdQuery(Guid.NewGuid()), 
            CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSessionLogs_WhenSessionLogsIsFound()
    {
        // Arrange
        var sessionLogs = FakeDataGenerator.GenerateFakeSessionLogs(10);
        var handler = new GetSessionLogsByIdQueryHandler(_sessionLogsRepository.Object);
        _sessionLogsRepository.Setup(x =>
                x.GetSessionLogsByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionLogs);
        
        // Act
        var result = await handler.Handle(
            new GetSessionLogsByIdQuery(Guid.NewGuid()), 
            CancellationToken.None);
        
        // Assert
        sessionLogs.Should().BeSameAs(result);
    }
}