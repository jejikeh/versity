using System.Reflection;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
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
            ? serviceCollection.AddSqliteDatabase(configuration.DatabaseConnectionString)
            : serviceCollection.AddPostgresDatabase(configuration.DatabaseConnectionString);
    }

    private static IServiceCollection AddSqliteDatabase(
        this IServiceCollection serviceCollection, 
        string? connectionString)
    {
        serviceCollection.AddDbContext<VersitySessionsServiceDbContext>(options =>
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
        serviceCollection.AddDbContext<VersitySessionsServiceDbContext>(options =>
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
        serviceCollection.AddScoped<ISessionsRepository, SessionsRepository>();
        serviceCollection.AddScoped<IProductsRepository, ProductRepository>();
        serviceCollection.AddScoped<ISessionLogsRepository, SessionLogsRepository>();
        serviceCollection.AddScoped<ILogsDataRepository, LogsDataRepository>();
        
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
                configuration.UsePostgreSqlStorage(applicationConfiguration.DatabaseConnectionString);
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