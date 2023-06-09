using System.Reflection;
using Application.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddDbContext<VersityUsersDbContext>(options =>
        {
            options.EnableDetailedErrors();
            options.UseNpgsql(
                configuration.GetConnectionString("VersityUsersDb"), 
                builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });

        serviceCollection.AddScoped<IVersityUsersRepository, VersityUsersRepository>();
        
        return serviceCollection;
    }
}