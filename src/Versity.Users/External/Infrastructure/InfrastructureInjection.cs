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
    public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("VersityUsersDb");
        if (string.IsNullOrEmpty(connectionString)) 
            connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            
        serviceCollection.AddDbContext<VersityUsersDbContext>(options =>
        {
            options.EnableDetailedErrors();
            options.UseNpgsql(
                connectionString, 
                builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });

        serviceCollection.AddScoped<IVersityUsersRepository, VersityUsersRepository>();
        serviceCollection.AddScoped<IVersityRolesRepository, VersityRoleRepository>();
        
        return serviceCollection;
    }

    public static async Task<IServiceProvider> EnsureRolesExists(this IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<IVersityRolesRepository>();
        var roles = Enum.GetNames(typeof(VersityRole));
        var anyRoleWasAdded = false;
        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role)) 
                continue;
            
            await roleManager.CreateRoleAsync(role);
            anyRoleWasAdded = true;
        }
        
        if (anyRoleWasAdded)
            await serviceProvider.GetRequiredService<VersityUsersDbContext>().SaveChangesAsync();
        
        return serviceProvider;
    }
}