using Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory)
    {
        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                
                return factory.Invoke();
            });
    }
}