﻿using Infrastructure.KafkaConsumerService.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.KafkaConsumerService;

public static class UseKafkaConsumerService
{
    public static IServiceCollection UseKafkaConsumer(this IServiceCollection serviceCollection,
        IKafkaConsumerConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddHostedService<KafkaProductConsumerService>();
        serviceCollection.AddScoped<IKafkaHandlersContainer, KafkaHandlersContainer>();

        return serviceCollection;
    }

    public static IServiceCollection AddKafkaHandler<T>(this IServiceCollection serviceCollection) where T : class, IKafkaMessageHandler
    {
        serviceCollection.AddScoped<IKafkaMessageHandler, T>();
        
        return serviceCollection;
    }
}