using Application;
using Application.Abstractions;
using Application.Abstractions.Hubs;
using Hangfire;
using Infrastructure;
using Infrastructure.Hubs;
using Infrastructure.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi.Models;
using Presentation.Configuration;
using Presentation.Hubs;
using Serilog;

namespace Presentation.Extensions;

public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDbContext(builder.Configuration)
            .AddRepositories()
            .AddRedisCaching()
            .AddApplication()
            .InjectSignalR()
            .AddHttpContextAccessor()
            .AddNotificationServices()
            .AddJwtAuthentication(builder.Configuration)
            .AddSwagger()
            .AddCors(options => options.ConfigureApiGatewayCors())
            .AddKafka(new KafkaConsumerConfiguration())
            .AddHangfireService()
            .AddEndpointsApiExplorer()
            .AddControllers();
        
        builder.Services.AddScoped<IVersityUsersDataService, GrpcUsersDataService>();
        
        return builder;
    }
    
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error-development");
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("AllowAll");
        app.UseHangfireDashboard();
        app.MapControllers();
        InfrastructureInjection.AddHangfireProcesses();
        app.MapHub<SessionsHub>("sessions-hub");
        
        return app;
    }
    
    private static CorsOptions ConfigureApiGatewayCors(this CorsOptions options)
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

    private static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
        });

        return serviceCollection;
    }

    private static IServiceCollection InjectSignalR(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSignalR(hubOptions =>
        {
            hubOptions.EnableDetailedErrors = true;
            hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
            hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(5);
        });
        
        serviceCollection.AddScoped<ISessionsHubHelper, SessionsHubHelper>();

        return serviceCollection;
    }
}