using Application.Abstractions.Repositories;
using Application.Common;
using Application.RequestHandlers.SessionLogging.Queries.GetAllLogsData;
using Application.RequestHandlers.SessionLogging.Queries.GetAllSessionsLogs;
using Domain.Models.SessionLogging;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.SessionLogging;

public class GetAllSessionsLogsTests
{
    private readonly Mock<ISessionLogsRepository> _sessionLogsRepository;

    public GetAllSessionsLogsTests()
    {
        _sessionLogsRepository = new Mock<ISessionLogsRepository>();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnLogs_WhenCalled()
    {
        // Arrange
        const int entriesCount = PageFetchSettings.ItemsOnPage + 5;
        var sessionLogs = FakeDataGenerator.GenerateFakeSessionsLogs(entriesCount, 10);
        var handler = new GetAllSessionsLogsQueryHandler(_sessionLogsRepository.Object);
        _sessionLogsRepository.Setup(repository => repository.GetAllSessionsLogs())
            .Returns(sessionLogs.AsQueryable());
        
        // Act
        await handler.Handle(new GetAllSessionsLogsQuery(2), CancellationToken.None);
        
        // Assert
        _sessionLogsRepository.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<SessionLogs>>(queryable => queryable.Count() == entriesCount - PageFetchSettings.ItemsOnPage)), 
            Times.Once()); 
    }
    
    [Fact]
    public async Task Handle_ShouldReturnsCorrectCountOfProducts_WhenCalledWithPage()
    {
        // Arrange
        var sessionLogs = FakeDataGenerator.GenerateFakeSessionsLogs(PageFetchSettings.ItemsOnPage + 5, 10);
        var handler = new GetAllSessionsLogsQueryHandler(_sessionLogsRepository.Object);
        _sessionLogsRepository.Setup(repository => repository.GetAllSessionsLogs())
            .Returns(sessionLogs.AsQueryable());
        
        // Act
        await handler.Handle(new GetAllSessionsLogsQuery(1), CancellationToken.None);
        
        // Assert
        _sessionLogsRepository.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<SessionLogs>>(queryable => queryable.Count() == PageFetchSettings.ItemsOnPage)), 
            Times.Once());  
    }
}