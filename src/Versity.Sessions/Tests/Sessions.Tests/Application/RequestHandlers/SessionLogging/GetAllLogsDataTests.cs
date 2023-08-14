using Application.Abstractions.Repositories;
using Application.Common;
using Application.RequestHandlers.SessionLogging.Queries.GetAllLogsData;
using Domain.Models.SessionLogging;
using FluentAssertions;
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
        _logsDataRepository.Setup(repository => repository.GetLogsData(
                It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(logsData);
        
        // Act
        var result = await handler.Handle(new GetAllLogsDataQuery(2), CancellationToken.None);
        
        // Assert
        result.Count().Should().Be(entriesCount);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnsCorrectCountOfProducts_WhenCalledWithPage()
    {
        // Arrange
        var logsData = FakeDataGenerator.GenerateFakeLogsData(PageFetchSettings.ItemsOnPage + 5);
        var handler = new GetAllLogsDataQueryHandler(_logsDataRepository.Object);
        _logsDataRepository.Setup(repository => repository.GetLogsData(
                It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(logsData);
        
        // Act
        var result = await handler.Handle(new GetAllLogsDataQuery(1), CancellationToken.None);
        
        // Assert
        result.Count().Should().Be(PageFetchSettings.ItemsOnPage + 5);
    }
}