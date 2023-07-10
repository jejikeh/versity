using Confluent.Kafka;

namespace Infrastructure.Services.KafkaConsumer.Abstractions;

public interface IKafkaConsumerConfiguration
{
    public string GroupId { get; set; }
    public string Host { get; set; }
    public string Topic { get; set; }
    public ConsumerConfig Config { get; set; }
}