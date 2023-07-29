using Infrastructure.Configurations;

namespace Presentation.Configuration;

public class KafkaProducerConfiguration : IKafkaProducerConfiguration
{
    public string ProducerName { get; } = Environment.GetEnvironmentVariable("KAFKA_ProducerName");
    public string CreateProductTopic { get; } = "CreateProduct";
    public string DeleteProductTopic { get; } = "DeleteProduct";
}