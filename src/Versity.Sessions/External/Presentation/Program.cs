using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
    var versitySessionsServiceDbContext = serviceProvider.GetRequiredService<VersitySessionsServiceDbContext>();
    versitySessionsServiceDbContext.Database.EnsureCreated();
    await app.RunAsync();
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Host terminated unexpectedly");
}