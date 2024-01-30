namespace Infrastructure.Configuration;

public static class DistributedCacheOptions
{
    public static TimeSpan CacheExpiryTime { get; set; } = TimeSpan.FromSeconds(60);
}