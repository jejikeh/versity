using Application.Abstractions.Repositories;
using Bogus;
using Infrastructure.Services.KafkaConsumer;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;
using Infrastructure.Services.KafkaConsumer.Handlers.DeleteProduct;
using Moq;

namespace Sessions.Tests.Infrastructure.Services.Kafka;

public class KafkaHandlerContainerTests
{
    public KafkaHandlerContainerTests()
    {
    }

    [Fact]
    public async Task ProcessMessage_ShouldCallAllHandlers_WhenHandlersAreSet()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var message = new Faker().Lorem.Sentence();
        var createProductMessageHandler = new Mock<IKafkaMessageHandler>();
        var deleteProductMessageHandler = new Mock<IKafkaMessageHandler>();
        
        var handlersContainer = new KafkaHandlersContainer(new List<IKafkaMessageHandler>()
        {
            createProductMessageHandler.Object,
            deleteProductMessageHandler.Object
        });
        
        // Act
        await handlersContainer.ProcessMessage(key, message, CancellationToken.None);
        
        // Assert
        createProductMessageHandler.Verify(x => x.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        deleteProductMessageHandler.Verify(x => x.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}