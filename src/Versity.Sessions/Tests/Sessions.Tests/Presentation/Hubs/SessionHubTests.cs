using Application.RequestHandlers.SessionLogging.Commands.CacheLogData;
using MediatR;
using Moq;
using Presentation.Hubs;
using Sessions.Tests.Application;

namespace Sessions.Tests.Presentation.Hubs;

public class SessionHubTests
{
    private readonly Mock<ISender> _sender;

    public SessionHubTests()
    {
        _sender = new Mock<ISender>();
    }

    [Fact]
    public async Task UploadStream_ShouldSendDataToMediator_WhenCommandIsValid()
    {
        // Arrange
        var count = new Random().Next(1, 10);
        var commands = FakeDataGenerator.GenerateAsyncFakeCacheLogDataCommands(count);
        var hub = new SessionsHub(_sender.Object);
        
        // Act
        await hub.UploadStream(commands);
        
        // Assert
        _sender.Verify(sender => sender.Send(It.IsAny<CacheLogDataCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(count));
    }
}