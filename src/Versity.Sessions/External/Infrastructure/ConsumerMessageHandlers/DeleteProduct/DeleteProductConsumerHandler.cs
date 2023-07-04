using Infrastructure.ConsumerMessageHandlers.CreateProduct;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ConsumerMessageHandlers.DeleteProduct;

public class DeleteProductConsumerHandler : IMessageHandler<DeleteProductDto>
{
    private readonly ILogger<CreateProductConsumerHandler> _logger;

    public DeleteProductConsumerHandler(ILogger<CreateProductConsumerHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(IMessageContext context, DeleteProductDto message)
    {
        _logger.LogInformation("The DeleteProductConsumerHandler was successful invoked!" +
                               $"The ID of deleted product - {message.Id}");

        return Task.CompletedTask;
    }
}