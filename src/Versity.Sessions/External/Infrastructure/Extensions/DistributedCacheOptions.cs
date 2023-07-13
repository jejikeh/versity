using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Extensions;

public static class DistributedCacheOptions
{
    public static DistributedCacheEntryOptions ConfigureCacheTimeOffsetExpire(TimeSpan timeSpan)
    {
        var options = new DistributedCacheEntryOptions();
        options.SetAbsoluteExpiration(timeSpan);
        
        return options;
    }
}