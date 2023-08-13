using System.Reflection;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.EmailServices;
using Infrastructure.Services.TokenServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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
        serviceCollection.AddDbContext<VersityUsersDbContext>(options =>
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
        serviceCollection.AddDbContext<VersityUsersDbContext>(options =>
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
        serviceCollection.AddScoped<IVersityUsersRepository, VersityUsersRepository>();
        serviceCollection.AddScoped<IVersityRolesRepository, VersityRoleRepository>();
        serviceCollection.AddScoped<IVersityRefreshTokensRepository, VersityRefreshTokensRepository>();
        
        return serviceCollection;
    }
    
    public static IServiceCollection AddServices(
        this IServiceCollection serviceCollection,
        IEmailServicesConfiguration emailServicesConfiguration, 
        ITokenGenerationConfiguration tokenGenerationConfiguration)
    {
        serviceCollection.UseEmailServices(emailServicesConfiguration);
        serviceCollection.UseTokenServices(tokenGenerationConfiguration);
        
        return serviceCollection;
    }
    
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection serviceCollection,
        IApplicationConfiguration configuration)
    {
        serviceCollection.Decorate<IVersityRefreshTokensRepository, CachedRefreshTokensRepository>();
        configuration.InjectCacheService(serviceCollection);
        
        return serviceCollection;
    }
}