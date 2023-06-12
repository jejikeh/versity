using System.Reflection;
using Application.Abstractions.Repositories;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
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
    
    public static async Task<IServiceProvider> CreateAdminUser(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var userManager = serviceProvider.GetRequiredService<IVersityUsersRepository>();
        if (await userManager.GetUserByIdAsync("admin") == null)
        {
            var adminUser = new VersityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                Email = "admin@admin.com"
            };
            
            // TODO: get password from env vars?
            await userManager.CreateUserAsync(adminUser, configuration.GetSection("Jwt:Issuer").Value);
            // TODO: add admin user to Admin Role. For now adding to role throw exception.
            await serviceProvider.GetRequiredService<VersityUsersDbContext>().SaveChangesAsync();
        }

        return serviceProvider;
    }
}