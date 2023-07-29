using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.SessionLogging.Queries.GetLogDataById;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.SessionLogging;

public class GetLogDataByIdTests
{
    private readonly Mock<ILogsDataRepository> _logsDataRepository;

    public GetLogDataByIdTests()
    {
        _logsDataRepository = new Mock<ILogsDataRepository>();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenLogDataIsNotFound()
    {
        // Arrange
        var handler = new GetLogDataByIdQueryHandler(_logsDataRepository.Object);
        _logsDataRepository.Setup(logsDataRepository => logsDataRepository.GetLogDataByIdAsync(
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as LogData);
        
        // Act
        var act = async () => await handler.Handle(
            new GetLogDataByIdQuery(Guid.NewGuid()), 
            CancellationToken.None);
        
        // Assert  
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnLog_WhenLogDataIsFound()
    {
        // Arrange
        var log = FakeDataGenerator.GenerateFakeLogData();
        var handler = new GetLogDataByIdQueryHandler(_logsDataRepository.Object);
        _logsDataRepository.Setup(logsDataRepository => logsDataRepository.GetLogDataByIdAsync(
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(log);
        
        // Act
        var result =  await handler.Handle(
            new GetLogDataByIdQuery(Guid.NewGuid()), 
            CancellationToken.None);
        
        // Assert  
        result.Should().BeSameAs(log);
    }
}