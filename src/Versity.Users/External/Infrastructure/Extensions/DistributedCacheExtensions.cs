using System.Text.Json;
using Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task<T?> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        VersityUsersDbContext dbContext,
        string key, 
        CancellationToken cancellationToken,
        Func<Task<T?>> factory) where T : class
    {
        T? member;
        var cachedMember = await cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedMember))
        {
            member = await factory.Invoke();

            if (member is null)
            {
                return member;
            }
            
            await cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(member),
                cancellationToken);

            return member;
        }
        
        member = JsonSerializer.Deserialize<T>(cachedMember);
        if (member is not null)
        {
            dbContext.Set<T>().Attach(member);
        }
        
        return member;
    }
}