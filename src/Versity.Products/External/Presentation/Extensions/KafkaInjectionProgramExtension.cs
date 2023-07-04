using System.Text.Json;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;

namespace Presentation.Extensions;

public static class KafkaInjectionProgramExtension
{
    public static IServiceCollection AddKafkaFlow(this IServiceCollection serviceCollection)
    {
        var kafkaServer = Environment.GetEnvironmentVariable("KAFKA_Host");
        var kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_Topic");
        var kafkaProducerName = Environment.GetEnvironmentVariable("KAFKA_ProducerName");

        serviceCollection.AddKafka(kafka => 
        {
            kafka
                .UseConsoleLog()
                .AddCluster(cluster =>  
                {
                    cluster
                        .WithBrokers(new[] { kafkaServer })
                        .CreateTopicIfNotExists(kafkaTopic, 1, 1)
                        .AddProducer(
                            kafkaProducerName,
                            producer => 
                            {
                                producer
                                    .DefaultTopic(kafkaTopic)
                                    .AddMiddlewares(middlewares => 
                                        middlewares.AddSerializer<JsonCoreSerializer>())
                                    .WithCompression(Confluent.Kafka.CompressionType.Gzip);
                            }
                        );
                });
        });

        return serviceCollection;
    }
}