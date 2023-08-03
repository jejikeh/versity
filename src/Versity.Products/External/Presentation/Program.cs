using Infrastructure.Persistence;
using Presentation.Extensions;

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
    var versityProductsDbContext = serviceProvider.GetRequiredService<VersityProductsDbContext>();
    versityProductsDbContext.Database.EnsureCreated();
    await app.RunAsync();
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Host terminated unexpectedly");
}

public partial class Program { }