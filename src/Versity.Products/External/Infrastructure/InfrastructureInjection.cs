using System.Reflection;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureInjection
{
    public static IServiceCollection AddDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        serviceCollection.AddDbContext<VersityProductsDbContext>(options =>
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
        serviceCollection.AddScoped<IVersityProductsRepository, VersityProductsRepository>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddKafkaServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IProductProducerService, KafkaProductProducerService>();

        return serviceCollection;
    }
    
    public static IServiceCollection AddRedisCaching(this IServiceCollection serviceCollection)
    {
        serviceCollection.Decorate<IVersityProductsRepository, CachedProductsRepository>();
        serviceCollection.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Environment.GetEnvironmentVariable("REDIS_Host");
        });
        
        return serviceCollection;
    }
}