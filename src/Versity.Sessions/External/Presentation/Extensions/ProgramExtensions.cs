using Application;
using Application.Abstractions;
using Application.Abstractions.Hubs;
using Hangfire;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Services.KafkaConsumer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Presentation.Configuration;
using Presentation.Hubs;

namespace Presentation.Extensions;

public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        var applicationConfiguration = new ApplicationConfiguration(builder.Configuration);
        var kafkaConsumerConfiguration = new KafkaConsumerConfiguration(builder.Configuration);
        
        builder.Services
            .AddDbContext(applicationConfiguration)
            .AddRepositories()
            .AddRedisCaching(applicationConfiguration)
            .AddApplication()
            .InjectSignalR()
            .AddHttpContextAccessor()
            .AddNotificationServices()
            .AddJwtAuthentication(builder.Configuration)
            .AddSwagger()
            .AddCors(options => options.ConfigureApiGatewayCors())
            .AddHangfireService(applicationConfiguration)
            .AddEndpointsApiExplorer()
            .AddControllers();

        if (!string.Equals(kafkaConsumerConfiguration.Host, "NO_SET"))
        {
            builder.Services.AddKafka(kafkaConsumerConfiguration);
        }
        else
        {
            builder.Services.AddFallbackKafkaServices(kafkaConsumerConfiguration);
        }
        
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
        
        // if we will be deploying in kubernetes, then https and ssl certificates
        // will be managing by kubernetes it self
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "false")
        {
            app.UseHttpsRedirection();
        }

        app.UseLoggingDependOnEnvironment();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("AllowAll");
        app.UseHangfireDashboard();
        app.MapControllers();
        InfrastructureInjection.AddHangfireProcesses();
        app.MapHub<SessionsHub>("sessions-hub");
        
        return app;
    }

    public static async Task<WebApplication> RunApplicationAsync(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        try
        {
            var versitySessionsServiceDbContext = serviceProvider.GetRequiredService<VersitySessionsServiceDbContext>();
            await versitySessionsServiceDbContext.Database.EnsureCreatedAsync();
            await versitySessionsServiceDbContext.Database.MigrateAsync();
            
            await webApplication.RunAsync();
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Host terminated unexpectedly");
        }

        return webApplication;
    }
    
    public static IServiceCollection AddFallbackKafkaServices(
        this IServiceCollection serviceCollection, 
        KafkaConsumerConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddHostedService<FallbackKafkaProductConsumerService>();

        return serviceCollection;
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