using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ConsumerMessageHandlers.CreateProduct;

public class CreateProductConsumerHandler : IMessageHandler<CreateProductDto>
{
    private readonly ILogger<CreateProductConsumerHandler> _logger;

    public CreateProductConsumerHandler(ILogger<CreateProductConsumerHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(IMessageContext context, CreateProductDto message)
    {
        _logger.LogInformation("The CreateProductConsumerHandler was successful invoked!" +
                               $"The title of created product - {message.Title}");

        return Task.CompletedTask;
    }
}