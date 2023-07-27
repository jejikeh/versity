using Infrastructure.Persistence;
using Presentation.Extensions;

var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureBuilder()
    .AddElasticAndSerilog();

var app = builder
    .Build()
    .ConfigureApplication();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
try
{
    var versityUsersDbContext = serviceProvider.GetRequiredService<VersityUsersDbContext>();
    versityUsersDbContext.Database.EnsureCreated();
    app.Run();
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Host terminated unexpectedly");
}

public partial class Program { }