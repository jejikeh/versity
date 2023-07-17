using System.Text.Json;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Extensions;

public static class DistributedCacheExtensions
{
    public static IQueryable<T> GetOrCreateQueryable<T>(
        this IDistributedCache cache, 
        string key,
        Func<IQueryable<T>> factory) where T : class
    {
        IQueryable<T> member;
        var cachedMember = cache.GetString(key);

        if (string.IsNullOrEmpty(cachedMember))
        {
            member = factory.Invoke();
            cache.SetString(
                key, 
                JsonSerializer.Serialize(member.ToList()),
                DistributedCacheOptions.ConfigureCacheTimeOffsetExpire(TimeSpan.FromMinutes(1)));
            
            return member;
        }

        member = JsonSerializer.Deserialize<List<T>>(cachedMember)!.AsQueryable();

        return member;
    }
    
    public static async Task<T?> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        VersitySessionsServiceDbContext dbContext,
        string key, 
        CancellationToken cancellationToken,
        Func<Task<T?>> factory) where T : class
    {
        var member = await cache.GetOrCreateAsync(key, cancellationToken, factory);
        if (member is not null && dbContext.Entry(member).State != EntityState.Modified)
        {
            dbContext.Set<T>().Attach(member);
        }

        return member;
    }
    
    public static async Task<T?> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        string key, 
        CancellationToken cancellationToken,
        Func<Task<T?>> factory) where T : class
    {
        var cachedMember = await cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(cachedMember))
        {
            return await cache.CreateAsync(key, cancellationToken, factory);
        }

        return JsonSerializer.Deserialize<T>(cachedMember);
    }
    
    public static async Task<T?> CreateAsync<T>(
        this IDistributedCache cache,
        string key, 
        CancellationToken cancellationToken,
        Func<Task<T?>> factory) where T : class
    {
        var member = await factory.Invoke();
        if (member is null)
        {
            return member;
        }

        await cache.SetStringAsync(key, JsonSerializer.Serialize(member), cancellationToken);

        return member;
    }
    
    public static async Task CreateAsync<T>(
        this IDistributedCache cache,
        string key, 
        CancellationToken cancellationToken,
        T member) where T : class
    {
        await cache.SetStringAsync(key, JsonSerializer.Serialize(member), cancellationToken);
    }
}