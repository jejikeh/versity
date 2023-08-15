using System.Reflection;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Hangfire;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Persistence.MongoRepositories;
using Infrastructure.Persistence.SqlRepositories;
using Infrastructure.Services;
using Infrastructure.Services.KafkaConsumer;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;
using Infrastructure.Services.KafkaConsumer.Handlers.DeleteProduct;
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
            ? serviceCollection.AddSqliteDatabase<VersitySessionsServiceSqlDbContext>(configuration.DatabaseConnectionString) 
            : serviceCollection.AddMongoDb();
    }

    private static IServiceCollection AddMongoDb(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<VersitySessionsServiceMongoDbContext>()
            .AddMongoRepositories();

        return serviceCollection;
    }
    
    private static IServiceCollection AddSqliteDatabase<T>(
        this IServiceCollection serviceCollection, 
        string? connectionString) where T : DbContext
    {
        serviceCollection.AddDbContext<T>(options =>
        {
            options.EnableDetailedErrors();
            options.UseSqlite(
                connectionString,
                builder =>
                {
                    builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                });
        });

        serviceCollection.AddSqlRepositories();

        return serviceCollection;
    }

    private static IServiceCollection AddPostgresDatabase<T>(
        this IServiceCollection serviceCollection, 
        string? connectionString) where T : DbContext
    {
        serviceCollection.AddDbContext<T>(options =>
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
    
    private static IServiceCollection AddMongoRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ISessionsRepository, SessionsMongoRepository>();
        serviceCollection.AddScoped<IProductsRepository, ProductMongoRepository>();
        serviceCollection.AddScoped<ISessionLogsRepository, SessionLogsMongoRepository>();
        serviceCollection.AddScoped<ILogsDataRepository, LogsDataMongoRepository>();
        
        return serviceCollection;
    }
    
    private static IServiceCollection AddSqlRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ISessionsRepository, SessionsSqlRepository>();
        serviceCollection.AddScoped<IProductsRepository, ProductSqlRepository>();
        serviceCollection.AddScoped<ISessionLogsRepository, SessionLogsSqlRepository>();
        serviceCollection.AddScoped<ILogsDataRepository, LogsDataSqlRepository>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddRedisCaching(
        this IServiceCollection serviceCollection,
        IApplicationConfiguration configuration)
    {
        serviceCollection.Decorate<ISessionsRepository, CachedSessionsRepository>();
        configuration.InjectCacheService(serviceCollection);
        
        return serviceCollection;
    }

    public static IServiceCollection AddKafka(
        this IServiceCollection serviceCollection, 
        IKafkaConsumerConfiguration configuration)
    {
        serviceCollection
            .AddKafkaHandler<CreateProductMessageHandler>()
            .AddKafkaHandler<DeleteProductMessageHandler>()
            .UseKafkaConsumer(configuration);
        
        return serviceCollection;
    }
    
    public static IServiceCollection AddHangfireService(
        this IServiceCollection serviceCollection,
        IApplicationConfiguration applicationConfiguration)
    {
        serviceCollection.AddHangfireServer();
        serviceCollection.AddHangfire(configuration =>
        {
            configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings();
            
            if (applicationConfiguration.IsDevelopmentEnvironment)
            {
                configuration.UseInMemoryStorage();
            }
            else
            {
                // configuration.UseMongoStorage(applicationConfiguration.DatabaseConnectionString);
                configuration.UseInMemoryStorage();
            }
        });

        serviceCollection.AddTransient<UpdateSessionStatusService>();
        serviceCollection.AddTransient<BackgroundWorkersCacheService>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddNotificationServices(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddScoped<INotificationSender, SessionNotificationsSenderService>();

        return servicesCollection;
    }

    public static void AddHangfireProcesses()
    {
        RecurringJob.AddOrUpdate<UpdateSessionStatusService>(
            "ExpireExpiredSessions",
            x => x.ExpireExpiredSessions(), 
            Cron.Minutely);
        
        RecurringJob.AddOrUpdate<UpdateSessionStatusService>(
            "OpenInactiveSessions",
            x => x.OpenInactiveSessions(), 
            Cron.Minutely);
        
        RecurringJob.AddOrUpdate<BackgroundWorkersCacheService>(
            "PushSessionsLogs",
            x => x.PushSessionLogs(), 
            Cron.Minutely);
    }
}