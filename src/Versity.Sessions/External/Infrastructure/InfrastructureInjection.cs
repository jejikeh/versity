using System.Reflection;
using Application.Abstractions.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services.KafkaConsumer;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;
using Infrastructure.Services.KafkaConsumer.Handlers.DeleteProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureInjection
{
    public static IServiceCollection AddDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        serviceCollection.AddDbContext<VersitySessionsServiceDbContext>(options =>
        {
            options.EnableDetailedErrors();
            options.UseNpgsql(
                connectionString,
                builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });

        return serviceCollection;
    }
    
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ISessionsRepository, SessionsRepository>();
        serviceCollection.AddScoped<IProductsRepository, ProductRepository>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddKafka(this IServiceCollection serviceCollection, IKafkaConsumerConfiguration configuration)
    {
        serviceCollection
            .AddKafkaHandler<CreateProductMessageHandler>()
            .AddKafkaHandler<DeleteProductMessageHandler>()
            .UseKafkaConsumer(configuration);
        
        return serviceCollection;
    }
}