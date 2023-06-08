using Microsoft.EntityFrameworkCore;
using Versity.Users.Core.Application.Abstractions;
using Versity.Users.Persistence.Repository;

namespace Versity.Users.Persistence;

public static class PersistenceInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddDbContext<VersityUsersDbContext>(options =>
        {
            options.EnableDetailedErrors();
            options.UseNpgsql(configuration.GetConnectionString("VersityUsersDb"));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });

        serviceCollection.AddScoped<IVersityUsersRepository, VersityUsersRepository>();
        
        return serviceCollection;
    }
}