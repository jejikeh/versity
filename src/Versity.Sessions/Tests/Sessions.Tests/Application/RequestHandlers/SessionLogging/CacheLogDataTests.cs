using Application.Abstractions;
using Application.Common;
using Application.RequestHandlers.SessionLogging.Commands.CacheLogData;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.SessionLogging;

public class CacheLogDataTests
{
    private readonly Mock<ICacheService> _cacheService;

    public CacheLogDataTests()
    {
        _cacheService = new Mock<ICacheService>();
    }

    [Fact]
    public async Task Handle_ShouldCacheData_WhenCalled()
    {
        // Arrange
        var command = FakeDataGenerator.GenerateFakeCacheLogDataCommand();
        var handler = new CacheLogDataCommandHandler(_cacheService.Object);
        
        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        _cacheService.Verify(x => x.SetAddAsync(
            CachingKeys.SessionLogs, 
            It.Is<CacheLogDataCommand>(x => x.Equals(command))), Times.Once);
    }
}