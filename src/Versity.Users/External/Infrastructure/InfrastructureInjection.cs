using System.Reflection;
using Application.Abstractions.Repositories;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureInjection
{
    public static IServiceCollection AddDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        serviceCollection.AddDbContext<VersityUsersDbContext>(options =>
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
        serviceCollection.AddScoped<IVersityUsersRepository, VersityUsersRepository>();
        serviceCollection.AddScoped<IVersityRolesRepository, VersityRoleRepository>();
        serviceCollection.AddScoped<IVersityRefreshTokensRepository, VersityRefreshTokensRepository>();
        
        return serviceCollection;
    }
}