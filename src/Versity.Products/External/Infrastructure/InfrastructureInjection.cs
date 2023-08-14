using System.Reflection;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureInjection
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection serviceCollection, 
        IApplicationConfiguration configuration)
    {
        return configuration.IsDevelopmentEnvironment
            ? serviceCollection.AddSqliteDatabase(configuration.DatabaseConnectionString)
            : serviceCollection.AddPostgresDatabase(configuration.DatabaseConnectionString);
    }
    
    private static IServiceCollection AddSqliteDatabase(
        this IServiceCollection serviceCollection, 
        string? connectionString)
    {
        serviceCollection.AddDbContext<VersityProductsDbContext>(options =>
        {
            options.EnableDetailedErrors();
            options.UseSqlite(
                connectionString,
                builder =>
                {
                    builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                });
        });

        return serviceCollection;
    }

    private static IServiceCollection AddPostgresDatabase(
        this IServiceCollection serviceCollection, 
        string? connectionString)
    {
        serviceCollection.AddDbContext<VersityProductsDbContext>(options =>
        {
            options.EnableDetailedErrors();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            options.UseNpgsql(
                connectionString,
                builder =>
                {
                    builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
        });

        return serviceCollection;
    }
    
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IVersityProductsRepository, VersityProductsRepository>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddKafkaServices(this IServiceCollection serviceCollection, IKafkaProducerConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddTransient<IProductProducerService, KafkaProductProducerService>();

        return serviceCollection;
    }
    
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection serviceCollection,
        IApplicationConfiguration configuration)
    {
        serviceCollection.Decorate<IVersityProductsRepository, CachedProductsRepository>();
        configuration.InjectCacheService(serviceCollection);
        
        return serviceCollection;
    }
}