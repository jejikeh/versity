﻿using System.Net;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Versity.ApiGateway.Configuration;

namespace Versity.ApiGateway.Extensions;

public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json")
            .AddEnvironmentVariables();

        builder.Services
            .AddJwtAuthentication(new TokenGenerationConfiguration(builder.Configuration))
            .AddCors(options => options.ConfigureFrontendCors(builder.Configuration))
            .AddOcelot(builder.Configuration);
        
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

        builder.Services.AddSignalR(configure =>
        {
        });
        
        return builder;
    }
    
    public static async Task<WebApplication> ConfigureApplication(this WebApplication app)
    {
        // if deploying in kubernetes, then https and ssl certificates
        // will be managing by kubernetes ingress
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "false")
        {
            app.UseHttpsRedirection();
        }
        
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseCors("AllowAll");
        app.UseWebSockets();
        await app.UseOcelot();

        return app;
    }

    public static async Task<WebApplication> RunApplicationAsync(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        try
        {
            await application.RunAsync();
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Host terminated unexpectedly");
        }
        
        return application;
    }
    
    private static CorsOptions ConfigureFrontendCors(this CorsOptions options, IConfiguration configuration)
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins(configuration["FrontendHost"] ?? throw new InvalidOperationException("Set the FrontendHost Variable"));
        });
        
        return options;
    }
}