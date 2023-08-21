using Confluent.Kafka;
using Infrastructure.Services.KafkaConsumer.Abstractions;

namespace Presentation.Configuration;

public class KafkaConsumerConfiguration : IKafkaConsumerConfiguration
{
    public string GroupId { get; set; }
    public string Host { get; set; }
    public string Topic { get; set; }
    public ConsumerConfig Config { get; set; }
    public string CreateProductTopic { get; } = "CreateProduct";
    public string DeleteProductTopic { get; } = "DeleteProduct";

    public KafkaConsumerConfiguration(IConfiguration configuration)
    {
        GroupId = "versity.sessions";
        Host = configuration["Kafka:Host"]
               ?? Environment.GetEnvironmentVariable("KAFKA_Host")
               ?? "NO_SET";
        Topic = configuration["Kafka:Topic"]
                ?? Environment.GetEnvironmentVariable("KAFKA_Topic") 
                ?? "NO_SET";
        
        Config = new ConsumerConfig()
        {
            BootstrapServers = Host,
            GroupId = GroupId,
            SecurityProtocol = SecurityProtocol.Plaintext,
            EnableAutoCommit = false,
            StatisticsIntervalMs = 5000,
            SessionTimeoutMs = 6000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true,
            AllowAutoCreateTopics = true
        };
    }
}