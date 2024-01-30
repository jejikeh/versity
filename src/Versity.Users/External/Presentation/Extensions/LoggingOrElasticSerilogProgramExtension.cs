using System.Reflection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Presentation.Extensions;

public static class LoggingOrElasticSerilogProgramExtension
{
    public static WebApplication UseLoggingDependOnEnvironment(this WebApplication application)
    {
        if (Environment.GetEnvironmentVariable("ElasticConfiguration:Uri") != "no_set")
        {
            application.UseSerilogRequestLogging();
        }
        
        return application;
    }
    
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        if (Environment.GetEnvironmentVariable("ElasticConfiguration:Uri") != "no_set")
        {
            return builder.AddElasticAndSerilog();
        }
        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddElasticAndSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environment))
            {
                throw new NullReferenceException("ASPNETCORE_ENVIRONMENT variable is empty!");
            }
            
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(configurationRoot, environment))
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .ReadFrom.Configuration(context.Configuration);
        });

        return builder;
    }
    
    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
    {
        var connectionString = configuration["ElasticConfiguration:Uri"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new NullReferenceException("ElasticConfiguration:Uri configuration is empty");
        }
        
        var connectionUri = new Uri(connectionString);

        return new ElasticsearchSinkOptions(connectionUri) 
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace('.', '-')}-{environment.ToString()}-{DateTime.UtcNow:yyyy-MM}",
            NumberOfReplicas = 1,
            NumberOfShards = 2,
        };
    }
}