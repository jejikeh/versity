using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Presentation.Configurations;

var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureBuilder();

var app = builder.Build();
app.ConfigureApplication();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
try
{
    var versityUsersDbContext = serviceProvider.GetRequiredService<VersityUsersDbContext>();
    versityUsersDbContext.Database.EnsureCreated();

    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = Enum.GetNames(typeof(VersityRole));
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}
catch (Exception _)
{
    // TODO
}

app.Run();