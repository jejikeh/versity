namespace Infrastructure.KafkaConsumerService.Abstractions;

public interface IKafkaHandlersContainer
{
    public Task ProcessMessage(string key, string message, CancellationToken cancellationToken);
}