using System.Text.Json;
using Application.Abstractions;
using Infrastructure.Configurations;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    
    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }
    
    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory)
    {
        var cachedMember = await _database.StringGetAsync(key);
        if (cachedMember.IsNullOrEmpty)
        {
            return await CreateAsync(key, factory);
        }

        return JsonSerializer.Deserialize<T>(cachedMember!);
    }
    
    private async Task<T?> CreateAsync<T>(string key, Func<Task<T?>> factory)
    {
        var value = await factory.Invoke();
        if (value is null)
        {
            return value;
        }
        
        await _database.StringSetAsync(key, JsonSerializer.Serialize(value), DistributedCacheOptions.CacheExpiryTime);
        
        return value;
    }
}