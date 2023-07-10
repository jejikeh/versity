namespace Infrastructure.Services.KafkaConsumer.Abstractions;

public interface IKafkaHandlersContainer
{
    public Task ProcessMessage(string key, string message, CancellationToken cancellationToken);
}