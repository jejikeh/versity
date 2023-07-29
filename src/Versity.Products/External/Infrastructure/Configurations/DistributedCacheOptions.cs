namespace Infrastructure.Configurations;

public static class DistributedCacheOptions
{
    public static TimeSpan CacheExpiryTime { get; } = TimeSpan.FromSeconds(60);
}