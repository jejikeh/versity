using Application.Abstractions;
using Infrastructure.Configuration;
using Infrastructure.Exceptions;
using Infrastructure.Services;
using StackExchange.Redis;

namespace Presentation.Configuration;

public class ApplicationConfiguration : IApplicationConfiguration
{
    public string DatabaseConnectionString { get; private set; } = string.Empty;
    public bool IsDevelopmentEnvironment { get; private set; }
    public string CacheServiceConnectionString { get; set; } = string.Empty;
    public string GrpcIdentityHost { get; set; }
    public string DatabaseName { get; set; } = string.Empty;

    public ApplicationConfiguration(IConfiguration configuration)
    {
        IsDevelopmentEnvironment = IsDevelopment();
        GrpcIdentityHost = configuration["GrpcIdentityHost"] ?? throw new UserSecretsInvalidException("GrpcIdentityHost");
        _ = SetupDbConnectionString(configuration) || FallBackToDevelopmentEnvironment();
    }

    public void InjectCacheService(IServiceCollection serviceCollection)
    {
        if (IsDevelopmentEnvironment)
        {
            serviceCollection.AddMemoryCache();
            serviceCollection.AddSingleton<ICacheService, InMemoryCacheService>();
            
            return;
        }

        serviceCollection.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(provider => 
            provider.GetService<ConnectionMultiplexer>() 
            ?? ConnectionMultiplexer.Connect(CacheServiceConnectionString));
        
        serviceCollection.AddSingleton<ICacheService, RedisCacheService>();
    }
    
    private bool SetupDbConnectionString(IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("SessionsDbContext");
        var cacheConnectionString = configuration.GetConnectionString("CacheDbContext");
        var databaseName = configuration.GetConnectionString("DatabaseName");

        // This is used for test environment. I could use appsettings.Integrations.json,
        // but im getting connection string to database in runtime at startup of integration tests
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEST_ConnectionString")))
        {
            cacheConnectionString = Environment.GetEnvironmentVariable("TEST_CacheHost");
            databaseName = Environment.GetEnvironmentVariable("TEST_DatabaseName");
            dbConnectionString = Environment.GetEnvironmentVariable("TEST_ConnectionString") + databaseName + "?authSource=admin";
        }

        if (string.IsNullOrEmpty(dbConnectionString) || 
            string.IsNullOrEmpty(cacheConnectionString) ||
            string.IsNullOrEmpty(databaseName))
        {
            return false;
        }

        DatabaseConnectionString =  dbConnectionString;
        CacheServiceConnectionString = cacheConnectionString;
        DatabaseName = databaseName;
        
        return true;
    }

    /// <summary>
    /// Force setup the Development environment if unable to setup proper environment.
    /// This means what the application will be using self deploy sqlite database.
    /// </summary>
    private bool FallBackToDevelopmentEnvironment()
    {
        Console.WriteLine("ERROR: UNABLE TO INITIALIZE ANY OF ENVIRONMENTS!");
        Console.WriteLine("FALLBACK TO FORCE DEVELOPMENT!");
        
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        IsDevelopmentEnvironment = true;
        DatabaseConnectionString = "Data Source=SessionsContext.db";
        DatabaseName = "SessionsContext";
        
        return true;
    }
    
    private static bool IsDevelopment()
    {
        return string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), 
            "development", 
            StringComparison.InvariantCultureIgnoreCase);
    }
}