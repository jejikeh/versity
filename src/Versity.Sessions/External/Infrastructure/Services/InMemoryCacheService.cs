using System.Numerics;
using System.Text.Json;
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

    public Task<T?> CreateAsync<T>(string key, Func<Task<T?>> factory)
    {
        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return factory.Invoke();
            });    }

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

    public Task SetAddRangeAsync<T>(string key, Func<IQueryable<T>> factory)
    {
        var values = factory.Invoke().ToArray();
        _memoryCache.Set(key, JsonSerializer.Serialize(values));

        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<T> GetSetOrAddRangeAsync<T>(string key, Func<IQueryable<T>> factory)
    {
        var values = _memoryCache.Get<IEnumerable<T>>(key);

        if (values is null)
        {
            await SetAddRangeAsync(key, factory);
            values = _memoryCache.Get<IEnumerable<T>>(key);
        }

        if (values != null)
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }
    }

    public async IAsyncEnumerable<T> GetSetAsync<T>(string key)
    {
        var values = _memoryCache.Get<IEnumerable<T>>(key);

        if (values is not null)
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }
    }

    public void SetRemoveMember<T>(string key, T member) where T : IEquatable<T>
    {
        var values = _memoryCache.Get<IEnumerable<T>>(key);

        // This workaround is to fix a strange bug when comparing the last object from InMemoryCache
        if (values?.Count() == 1)
        {
            _memoryCache.Set(key, new List<T>() { });
            return;
        }
        
        if (values is null)
        {
            return;
        }
        
        var valueArray = values as T[] ?? values.Where(value => value.Equals(member)).ToArray();
        _memoryCache.Set(key, valueArray);
    }

    public async Task SetAddAsync<T>(string key, Func<Task<T?>> factory)
    {
        var values = _memoryCache.Get<IEnumerable<T>>(key);

        if (values is not null)
        {
            var valueArray = values.ToList();
            var insertValue = await factory.Invoke();

            if (insertValue is not null)
            {
                valueArray.Add(insertValue);
            }
            
            _memoryCache.Set(key, valueArray);
        }
    }

    public Task SetAddAsync<T>(string key, T obj)
    {
        Console.WriteLine(key);
        var values = _memoryCache.Get<IEnumerable<T>>(key);

        if (values is not null)
        {
            var valueArray = values.ToList();
            valueArray.Add(obj);

            _memoryCache.Set(key, valueArray);
        }
        else
        {
            _memoryCache.Set(key, new List<T>() { obj });
        }
        
        return Task.CompletedTask;
    }

    public IQueryable<T> GetSetOrAddRangeQueryableAsync<T>(string key, Func<IQueryable<T>> factory)
    {
        var collection = GetSetOrAddRangeAsync(key, factory);
        
        return collection.ToBlockingEnumerable().AsQueryable();    
    }
}