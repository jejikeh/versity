using KafkaFlow;
using KafkaFlow.Configuration;

namespace Presentation.Extensions;

public static class KafkaInjectionProgramExtension
{
    public static IServiceCollection AddKafkaFlow(this IServiceCollection serviceCollection)
    {
        var kafkaServer = Environment.GetEnvironmentVariable("KAFKA_Host");
        var kafkaUsername = Environment.GetEnvironmentVariable("KAFKA_Username");
        var kafkaPassword = Environment.GetEnvironmentVariable("KAFKA_Password");
        var kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_Topic");

        serviceCollection.AddKafka(kafka => 
        {
            kafka
                .UseConsoleLog()
                .AddCluster(cluster =>  
                {
                    cluster
                        .WithBrokers(new[] { kafkaServer })
                        .WithSecurityInformation(information => 
                        {
                            information.SaslMechanism = SaslMechanism.Plain;
                            information.SaslUsername = kafkaUsername;
                            information.SaslPassword = kafkaPassword;
                            information.SecurityProtocol = SecurityProtocol.Plaintext;
                            information.EnableSslCertificateVerification = true;
                        })
                        .AddProducer(
                            "versity.products.dev",
                            producer => 
                            {
                                producer
                                    .DefaultTopic(kafkaTopic)
                                    .WithCompression(Confluent.Kafka.CompressionType.Gzip);
                            }
                        );
                });
        });

        return serviceCollection;
    }
}