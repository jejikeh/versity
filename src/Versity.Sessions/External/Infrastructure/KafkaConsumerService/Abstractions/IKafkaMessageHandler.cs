using Confluent.Kafka;

namespace Infrastructure.KafkaConsumerService.Abstractions;

public interface IKafkaMessageHandler
{
    public Task Handle(string key, string message, CancellationToken cancellationToken);
}