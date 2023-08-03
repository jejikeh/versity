using Confluent.Kafka;

namespace Products.Tests.Integrations.Helpers.KafkaConsumer.Abstractions;

public interface IKafkaConsumerConfiguration
{
    public string GroupId { get; set; }
    public string Host { get; set; }
    public string Topic { get; set; }
    public ConsumerConfig Config { get; set; }
    public string CreateProductTopic { get; }
    public string DeleteProductTopic { get; }
}