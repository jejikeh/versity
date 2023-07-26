using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Sessions.Commands.DeleteSession;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Sessions;

public class DeleteSessionTests
{
    private readonly Mock<ISessionsRepository> _sessions;
    private readonly DeleteSessionCommandHandler _deleteSessionCommandHandler;
    
    public DeleteSessionTests()
    {
        _sessions = new Mock<ISessionsRepository>();
        _deleteSessionCommandHandler = new DeleteSessionCommandHandler(_sessions.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenSessionIsNotFound()
    {
        // Arrange
        _sessions.Setup(x => x.GetSessionByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Session);
        
        var act = async () => await _deleteSessionCommandHandler.Handle(new DeleteSessionCommand(Guid.NewGuid()), CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldDeleteSessionAndSaveChanges_WhenSessionIsFound()
    {
        // Arrange
        _sessions.Setup(x => x.GetSessionByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        await _deleteSessionCommandHandler.Handle(new DeleteSessionCommand(Guid.NewGuid()), CancellationToken.None);
        
        // Assert
        _sessions.Verify(x => x.DeleteSession(It.IsAny<Session>()), Times.Once);
        _sessions.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}