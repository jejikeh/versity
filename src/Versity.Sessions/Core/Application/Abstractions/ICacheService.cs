﻿using Microsoft.Extensions.Caching.Memory;

namespace Application.Abstractions;

public interface ICacheService
{
    public Task<T?> CreateAsync<T>(string key, Func<Task<T?>> factory);
    public Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory);
    public Task SetAddRangeAsync<T>(string key, Func<IQueryable<T>> factory);
    public IAsyncEnumerable<T> GetSetOrAddRangeAsync<T>(string key, Func<IQueryable<T>> factory);
    public IAsyncEnumerable<T> GetSetAsync<T>(string key);
    public void SetRemoveMember<T>(string key, T member) where T :  IEquatable<T>;
    public Task SetAddAsync<T>(string key, Func<Task<T?>> factory);
    public Task SetAddAsync<T>(string key, T obj);
    public IQueryable<T> GetSetOrAddRangeQueryableAsync<T>(string key, Func<IQueryable<T>> factory);
}