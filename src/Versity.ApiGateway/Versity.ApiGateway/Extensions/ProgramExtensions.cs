using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
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
        
        builder.Services.AddOcelot(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddCors(options => options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        }));
        
        return builder;
    }
    
    public static async Task<WebApplication> ConfigureApplication(this WebApplication app)
    {
        await app.UseOcelot();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseCors("AllowAll");

        return app;
    }
}