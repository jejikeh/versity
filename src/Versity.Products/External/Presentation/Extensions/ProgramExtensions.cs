using Application;
using Application.Abstractions;
using Infrastructure;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi.Models;
using Presentation.Configuration;
using Serilog;

namespace Presentation.Extensions;

public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        var applicationConfiguration = new ApplicationConfiguration(builder.Configuration);
        var tokenGenerationConfiguration = new TokenGenerationConfiguration(builder.Configuration);
        var kafkaProducerConfiguration = new KafkaProducerConfiguration(builder.Configuration);
        
        builder.Services
            .AddDbContext(applicationConfiguration)
            .AddRepositories()
            .AddRedisCaching(applicationConfiguration)
            .AddApplication()
            .AddJwtAuthentication(tokenGenerationConfiguration)
            .AddSwagger()
            .AddCors(options => options.ConfigureAllowAllCors())
            .AddEndpointsApiExplorer()
            .AddControllers();

        if (!string.Equals(kafkaProducerConfiguration.ProducerName, "NO_SET"))
        {
            builder.Services
                .AddKafkaServices(kafkaProducerConfiguration)
                .AddKafkaFlow(kafkaProducerConfiguration);
        }
        else
        {
            builder.Services.AddFallbackKafkaServices(kafkaProducerConfiguration);
        }
        
        builder.AddLoggingServices(applicationConfiguration);
        
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
        app.MapControllers();
        
        return app;
    }
    
    public static async Task<WebApplication> RunApplicationAsync(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        try
        {
            var versityProductsDbContext = serviceProvider.GetRequiredService<VersityProductsDbContext>();
            await versityProductsDbContext.Database.EnsureCreatedAsync();
            
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
        IKafkaProducerConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddTransient<IProductProducerService, FallbackKafkaProductProducerService>();

        return serviceCollection;
    }
    
    private static CorsOptions ConfigureAllowAllCors(this CorsOptions options)
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
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
}