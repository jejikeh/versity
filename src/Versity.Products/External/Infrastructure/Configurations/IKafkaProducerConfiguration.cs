using Confluent.Kafka;

namespace Infrastructure.Configurations;

public interface IKafkaProducerConfiguration
{
    public string ProducerName { get; }
    public string CreateProductTopic { get; }
    public string DeleteProductTopic { get; }
}