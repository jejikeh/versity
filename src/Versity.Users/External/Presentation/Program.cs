using Infrastructure;
using Infrastructure.Persistence;
using Presentation.Extensions;

var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureBuilder();

var app = builder
    .Build()
    .ConfigureApplication();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
try
{
    var versityUsersDbContext = serviceProvider.GetRequiredService<VersityUsersDbContext>();
    versityUsersDbContext.Database.EnsureCreated();

    await serviceProvider.EnsureRolesExists();
    await serviceProvider.CreateAdminUser(builder.Configuration);
    
    app.Run();
}
catch (Exception _)
{
    // TODO
}