using Application.Abstractions.Repositories;
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
        var sessionLogs = FakeDataGenerator.GenerateFakeSessionsLogs(15, 10);
        var handler = new GetAllSessionsLogsQueryHandler(_sessionLogsRepository.Object);
        _sessionLogsRepository.Setup(repository => repository.GetAllSessionsLogs())
            .Returns(sessionLogs.AsQueryable());
        
        // Act
        await handler.Handle(new GetAllSessionsLogsQuery(2), CancellationToken.None);
        
        // Assert
        _sessionLogsRepository.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<SessionLogs>>(queryable => queryable.Count() == 5)), 
            Times.Once()); 
    }
    
    [Fact]
    public async Task Handle_ShouldReturnsCorrectCountOfProducts_WhenCalledWithPage()
    {
        // Arrange
        var sessionLogs = FakeDataGenerator.GenerateFakeSessionsLogs(20, 10);
        var handler = new GetAllSessionsLogsQueryHandler(_sessionLogsRepository.Object);
        _sessionLogsRepository.Setup(repository => repository.GetAllSessionsLogs())
            .Returns(sessionLogs.AsQueryable());
        
        // Act
        await handler.Handle(new GetAllSessionsLogsQuery(2), CancellationToken.None);
        
        // Assert
        _sessionLogsRepository.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<SessionLogs>>(queryable => queryable.Count() == 10)), 
            Times.Once());  
    }
}