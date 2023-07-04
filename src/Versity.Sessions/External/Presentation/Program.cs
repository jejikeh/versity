using Infrastructure.Persistence;
using KafkaFlow;
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
    var versityUsersDbContext = serviceProvider.GetRequiredService<VersitySessionsServiceDbContext>();
    versityUsersDbContext.Database.EnsureCreated();

    var kafkaBus = app.Services.CreateKafkaBus();
    await kafkaBus.StartAsync();
    
    await app.RunAsync();
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Host terminated unexpectedly");
}