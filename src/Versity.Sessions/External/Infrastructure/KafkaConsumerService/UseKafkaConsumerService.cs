using Infrastructure.KafkaConsumerService.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.KafkaConsumerService;

public static class UseKafkaConsumerService
{
    public static IServiceCollection UseKafkaConsumer(this IServiceCollection serviceCollection,
        IKafkaConsumerConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddSingleton<IKafkaHandlersContainer, KafkaHandlersContainer>();
        serviceCollection.AddHostedService<KafkaProductConsumerService>();

        return serviceCollection;
    }

    public static IServiceCollection AddKafkaHandler<T>(this IServiceCollection serviceCollection) where T : class, IKafkaMessageHandler
    {
        serviceCollection.AddSingleton<IKafkaMessageHandler, T>();
        
        return serviceCollection;
    }
}