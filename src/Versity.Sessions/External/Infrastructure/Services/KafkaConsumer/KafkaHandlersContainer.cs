using Infrastructure.Services.KafkaConsumer.Abstractions;

namespace Infrastructure.Services.KafkaConsumer;

public class KafkaHandlersContainer : IKafkaHandlersContainer
{
    private readonly IEnumerable<IKafkaMessageHandler> _messageHandlers;
    
    public KafkaHandlersContainer(IEnumerable<IKafkaMessageHandler> messageHandlers)
    {
        _messageHandlers = messageHandlers;
    }

    public async Task ProcessMessage(string key, string message, CancellationToken cancellationToken)
    {
        foreach (var messageHandler in _messageHandlers)
        {
            Console.WriteLine($"[{key}] {message}");
            await messageHandler.Handle(key, message, cancellationToken);
        }
    }
}