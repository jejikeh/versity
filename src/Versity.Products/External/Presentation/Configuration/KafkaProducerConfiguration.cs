using Infrastructure.Configurations;

namespace Presentation.Configuration;

public class KafkaProducerConfiguration : IKafkaProducerConfiguration
{
    public string ProducerName { get; }
    public string CreateProductTopic { get; } = "CreateProduct";
    public string DeleteProductTopic { get; } = "DeleteProduct";
    public string Host { get; }
    public string Topic { get; }

public KafkaProducerConfiguration(IConfiguration configuration)
    {
        ProducerName = configuration["Kafka:ProducerName"] 
                       ?? Environment.GetEnvironmentVariable("KAFKA_ProducerName") 
                       ?? "NO_SET";

        Host = configuration["Kafka:Host"]
               ?? Environment.GetEnvironmentVariable("KAFKA_Host")
               ?? "NO_SET";

        Topic = configuration["Kafka:Topic"]
                ?? Environment.GetEnvironmentVariable("KAFKA_Topic")
                ?? "NO_SET";
    }
}