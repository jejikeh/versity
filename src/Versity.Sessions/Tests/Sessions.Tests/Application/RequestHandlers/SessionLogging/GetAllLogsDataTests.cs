using Application.Abstractions.Repositories;
using Application.Common;
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
        const int entriesCount = PageFetchSettings.ItemsOnPage + 5;
        var logsData = FakeDataGenerator.GenerateFakeLogsData(entriesCount);
        var handler = new GetAllLogsDataQueryHandler(_logsDataRepository.Object);
        _logsDataRepository.Setup(repository => repository.GetAllLogsData())
            .Returns(logsData.AsQueryable);
        
        // Act
        await handler.Handle(new GetAllLogsDataQuery(2), CancellationToken.None);
        
        _logsDataRepository.Verify(repository => 
            repository.ToListAsync(
                It.Is<IQueryable<LogData>>(queryable => queryable.Count() == entriesCount - PageFetchSettings.ItemsOnPage)), 
            Times.Once()); 
    }
    
    [Fact]
    public async Task Handle_ShouldReturnsCorrectCountOfProducts_WhenCalledWithPage()
    {
        // Arrange
        var logsData = FakeDataGenerator.GenerateFakeLogsData(PageFetchSettings.ItemsOnPage + 5);
        var handler = new GetAllLogsDataQueryHandler(_logsDataRepository.Object);
        _logsDataRepository.Setup(repository => repository.GetAllLogsData())
            .Returns(logsData.AsQueryable);
        
        // Act
        await handler.Handle(new GetAllLogsDataQuery(1), CancellationToken.None);
        
        _logsDataRepository.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<LogData>>(queryable => queryable.Count() == PageFetchSettings.ItemsOnPage)), 
            Times.Once()); 
    }
}