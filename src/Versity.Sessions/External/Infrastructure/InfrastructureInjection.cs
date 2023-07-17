using System.Reflection;
using Application.Abstractions;
using Application.Abstractions.Notifications;
using Application.Abstractions.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.KafkaConsumer;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;
using Infrastructure.Services.KafkaConsumer.Handlers.DeleteProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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
        serviceCollection.AddScoped<ISessionLogsRepository, SessionLogsRepository>();
        serviceCollection.AddScoped<ILogsDataRepository, LogsDataRepository>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddRedisCaching(this IServiceCollection serviceCollection)
    {
        serviceCollection.Decorate<ISessionsRepository, CachedSessionsRepository>();
        serviceCollection.AddSingleton(ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_Host")));
        serviceCollection.AddSingleton<ICacheService, RedisCacheService>();
        
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
    
    public static IServiceCollection AddHangfireService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHangfireServer();
        serviceCollection.AddHangfire(configuration => configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(Environment.GetEnvironmentVariable("ConnectionString")));

        serviceCollection.AddTransient<UpdateSessionStatusService>();
        
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
        
        RecurringJob.AddOrUpdate<UpdateSessionStatusService>(
            "PushSessionsLogs",
            x => x.PushSessionLogs(), 
            Cron.Minutely);
    }
}