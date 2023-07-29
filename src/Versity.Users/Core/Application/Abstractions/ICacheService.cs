namespace Application.Abstractions;

public interface ICacheService
{
    public Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory);
}