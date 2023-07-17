using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Extensions;

public static class DistributedCacheOptions
{
    public static TimeSpan CacheExpiryTime { get; set; } = TimeSpan.FromSeconds(60);
}