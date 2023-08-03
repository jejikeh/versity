using Microsoft.Extensions.DependencyInjection;
using Products.Tests.Integrations.Helpers.KafkaConsumer.Abstractions;

namespace Products.Tests.Integrations.Helpers.KafkaConsumer;

public static class UseKafkaConsumerService
{
    public static IServiceCollection UseKafkaConsumer(this IServiceCollection serviceCollection,
        IKafkaConsumerConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddSingleton<KafkaProductConsumerService>();

        return serviceCollection;
    }
}