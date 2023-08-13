using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations;

public interface IApplicationConfiguration
{
    public string DatabaseConnectionString { get; }
    public bool IsDevelopmentEnvironment { get; }
    public string CacheServiceConnectionString { get; set; }
    public void InjectCacheService(IServiceCollection serviceCollection);
    public Ports OpenPorts { get; }
    
    public class Ports
    {
        public int HttpsPort { get; set; }
        public int HttpPort { get; set; }
    }
}