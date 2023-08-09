using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Presentation.Extensions;

namespace Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .ConfigureBuilder()
            .AddLogging();

        var app = builder
            .Build()
            .ConfigureApplication();

        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        try
        {
            var versitySessionsServiceDbContext = serviceProvider.GetRequiredService<VersitySessionsServiceDbContext>();
            versitySessionsServiceDbContext.Database.EnsureCreated();
            await versitySessionsServiceDbContext.Database.MigrateAsync();
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Host terminated unexpectedly");
        }
    }
}