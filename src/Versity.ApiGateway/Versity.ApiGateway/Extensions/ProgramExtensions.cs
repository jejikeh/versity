using Microsoft.AspNetCore.Cors.Infrastructure;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Versity.ApiGateway.Extensions;

public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services
            .AddJwtAuthentication(builder.Configuration)
            .AddCors(options => options.ConfigureFrontendCors())
            .AddOcelot(builder.Configuration);

        builder.Services.AddSignalR();
        
        return builder;
    }
    
    public static async Task<WebApplication> ConfigureApplication(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseCors("AllowAll");
        app.UseWebSockets();
        await app.UseOcelot();

        return app;
    }
    
    private static CorsOptions ConfigureFrontendCors(this CorsOptions options)
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:3000");
        });
        
        return options;
    }
}