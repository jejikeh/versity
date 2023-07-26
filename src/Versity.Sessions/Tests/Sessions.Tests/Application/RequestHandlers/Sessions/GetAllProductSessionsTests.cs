using Application.Abstractions.Repositories;
using Application.Common;
using Application.RequestHandlers.SessionLogging.Queries.GetAllSessionsLogs;
using Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;
using Domain.Models;
using Domain.Models.SessionLogging;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Sessions;

public class GetAllProductSessionsTests
{
    private readonly Mock<ISessionsRepository> _sessions;

    public GetAllProductSessionsTests()
    {
        _sessions = new Mock<ISessionsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnProductSessions_WhenCalled()
    {
        // Arrange
        var entriesCount = PageFetchSettings.ItemsOnPage + 5;
        var sessionLogs = FakeDataGenerator.GenerateFakeSessions(entriesCount, 10);
        var handler = new GetAllProductSessionsQueryHandler(_sessions.Object);
        _sessions.Setup(repository => repository.GetAllProductSessions(It.IsAny<Guid>()))
            .Returns(sessionLogs.AsQueryable());

        _sessions.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(sessionLogs);
        
        // Act
        await handler.Handle(new GetAllProductSessionsQuery(Guid.NewGuid(), 2), CancellationToken.None);
        
        // Assert
        _sessions.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<Session>>(queryable => queryable.Count() == entriesCount - PageFetchSettings.ItemsOnPage)), 
            Times.Once()); 
    }
    
    [Fact]
    public async Task Handle_ShouldReturnCorrectCountOfSession_WhenCalledWithPage()
    {
        // Arrange
        var sessionLogs = FakeDataGenerator.GenerateFakeSessions(PageFetchSettings.ItemsOnPage + 5, 10);
        var handler = new GetAllProductSessionsQueryHandler(_sessions.Object);
        _sessions.Setup(repository => repository.GetAllProductSessions(It.IsAny<Guid>()))
            .Returns(sessionLogs.AsQueryable());

        _sessions.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(sessionLogs);
        
        // Act
        await handler.Handle(new GetAllProductSessionsQuery(Guid.NewGuid(), 1), CancellationToken.None);
        
        // Assert
        _sessions.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<Session>>(queryable => queryable.Count() == PageFetchSettings.ItemsOnPage)), 
            Times.Once()); 
    }
}