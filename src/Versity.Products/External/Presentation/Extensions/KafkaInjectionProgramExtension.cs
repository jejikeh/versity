using System.Text.Json;
using Infrastructure.Configurations;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;

namespace Presentation.Extensions;

public static class KafkaInjectionProgramExtension
{
    public static IServiceCollection AddKafkaFlow(
        this IServiceCollection serviceCollection,
        IKafkaProducerConfiguration kafkaProducerConfiguration)
    {
        serviceCollection.AddKafka(kafka => 
        {
            kafka
                .UseConsoleLog()
                .AddCluster(cluster =>  
                {
                    cluster
                        .WithBrokers(new[] { kafkaProducerConfiguration.Host })
                        .CreateTopicIfNotExists(kafkaProducerConfiguration.DeleteProductTopic, 1, 1)
                        .AddProducer(
                            kafkaProducerConfiguration.ProducerName,
                            producer =>
                            {
                                producer
                                    .DefaultTopic(kafkaProducerConfiguration.Topic)
                                    .AddMiddlewares(middlewares =>
                                        middlewares.AddSerializer<JsonCoreSerializer>());
                            }
                        );
                });
        });
        
        return serviceCollection;
    }
}