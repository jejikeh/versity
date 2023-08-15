using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations;

public interface IApplicationConfiguration
{
    public string DatabaseConnectionString { get; }
    public string DatabaseName { get; set; }
    public bool IsDevelopmentEnvironment { get; }
    public string CacheServiceConnectionString { get; set; }
    public void InjectCacheService(IServiceCollection serviceCollection);
}