using Application.Abstractions.Repositories;
using Application.RequestHandlers.SessionLogging.Queries.GetAllLogsData;
using Domain.Models.SessionLogging;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.SessionLogging;

public class GetAllLogsDataTests
{
    private readonly Mock<ILogsDataRepository> _logsDataRepository;

    public GetAllLogsDataTests()
    {
        _logsDataRepository = new Mock<ILogsDataRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnLogs_WhenCalled()
    {
        // Arrange
        var logsData = FakeDataGenerator.GenerateFakeLogsData(15);
        var handler = new GetAllLogsDataQueryHandler(_logsDataRepository.Object);
        _logsDataRepository.Setup(repository => repository.GetAllLogsData())
            .Returns(logsData.AsQueryable);
        
        // Act
        await handler.Handle(new GetAllLogsDataQuery(2), CancellationToken.None);
        
        _logsDataRepository.Verify(repository => 
            repository.ToListAsync(
                It.Is<IQueryable<LogData>>(queryable => queryable.Count() == 5)), 
            Times.Once()); 
    }
    
    [Fact]
    public async Task Handle_ShouldReturnsCorrectCountOfProducts_WhenCalledWithPage()
    {
        // Arrange
        var logsData = FakeDataGenerator.GenerateFakeLogsData(15);
        var handler = new GetAllLogsDataQueryHandler(_logsDataRepository.Object);
        _logsDataRepository.Setup(repository => repository.GetAllLogsData())
            .Returns(logsData.AsQueryable);
        
        // Act
        await handler.Handle(new GetAllLogsDataQuery(1), CancellationToken.None);
        
        _logsDataRepository.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<LogData>>(queryable => queryable.Count() == 10)), 
            Times.Once()); 
    }
}