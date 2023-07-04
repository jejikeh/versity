using Infrastructure.ConsumerMessageHandlers;
using Infrastructure.ConsumerMessageHandlers.CreateProduct;
using Infrastructure.ConsumerMessageHandlers.DeleteProduct;
using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;
using SaslMechanism = KafkaFlow.Configuration.SaslMechanism;
using SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol;

namespace Presentation.Extensions;

public static class KafkaInjectionProgramExtension
{
    public static IServiceCollection AddKafkaFlow(this IServiceCollection serviceCollection)
    {
        var kafkaServer = Environment.GetEnvironmentVariable("KAFKA_Host");
        var kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_Topic");

        serviceCollection.AddKafka(kafka => 
        {
            kafka
                .UseConsoleLog()
                .AddCluster(cluster =>  
                {
                    cluster
                        .WithBrokers(new[] { kafkaServer })
                        .CreateTopicIfNotExists(kafkaTopic, 1, 1)
                        .AddConsumer(consumer =>
                            {
                                consumer
                                    .Topic(kafkaTopic)
                                    .WithGroupId("versity-sessions-dev")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(3)
                                    .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                                    .AddMiddlewares(middlewares =>
                                    {
                                        middlewares
                                            .AddSerializer<JsonCoreSerializer>()
                                            .AddTypedHandlers(handler =>
                                                handler
                                                    .AddHandler<CreateProductConsumerHandler>()
                                                    .AddHandler<DeleteProductConsumerHandler>());
                                    });
                            }
                        );
                });
        });

        return serviceCollection;
    }
}