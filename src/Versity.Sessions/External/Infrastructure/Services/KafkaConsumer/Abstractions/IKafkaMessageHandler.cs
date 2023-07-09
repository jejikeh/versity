namespace Infrastructure.Services.KafkaConsumer.Abstractions;

public interface IKafkaMessageHandler
{
    public Task Handle(string key, string message, CancellationToken cancellationToken);
}