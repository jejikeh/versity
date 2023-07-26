using Application.RequestHandlers.SessionLogging.Commands.CacheLogData;
using Bogus;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sessions.Tests.Application;
using StackExchange.Redis;

namespace Sessions.Tests.Infrastructure.Services;

public class RedisCacheServiceTests
{
    private readonly Mock<IDatabase> _database;
    private readonly Mock<IConnectionMultiplexer> _connectionMultiplexer;
    private readonly Faker _faker = new Faker();
    private readonly RedisCacheService _redisCacheService;

    public RedisCacheServiceTests()
    {
        _database = new Mock<IDatabase>();
        _connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        
        _connectionMultiplexer.Setup(
                x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_database.Object);
        
        _redisCacheService = new RedisCacheService(_connectionMultiplexer.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnValueAndCacheIt_WhenValueIsNotInCache()
    {
        // Arrange
        var key = _faker.Lorem.Word();
        var factory = new Func<Task<CacheLogDataCommand>>(() => Task.Run(FakeDataGenerator.GenerateFakeCacheLogDataCommand));
        
        // Act
        var result = await _redisCacheService.CreateAsync(key, factory!);
        
        // Assert
        _database.Verify(database => database.StringSetAsync(
                It.Is<RedisKey>(redisKey => redisKey.Equals(key)), 
                It.IsAny<RedisValue>(), 
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()), 
            Times.Once());
    }
    
    [Fact]
    public void SetRemoveMember_ShouldRemoveValueFromCache_WhenValueIsInCache()
    {
        // Arrange
        var key = _faker.Lorem.Word();
        
        // Act
        _redisCacheService.SetRemoveMember(key, FakeDataGenerator.GenerateFakeCacheLogDataCommand());
        
        // Assert
        _database.Verify(database => database.SetRemove(
                It.Is<RedisKey>(redisKey => redisKey.Equals(key)), 
                It.IsAny<RedisValue>(), 
                It.IsAny<CommandFlags>()), 
            Times.Once());
    }
    
    [Fact]
    public async Task SetAddAsync_ShouldCacheObjectInSet_WhenValueIsInNotNull()
    {
        // Arrange
        var key = _faker.Lorem.Word();
        var factory = new Func<Task<CacheLogDataCommand>>(() => Task.Run(FakeDataGenerator.GenerateFakeCacheLogDataCommand));
        
        // Act
        await _redisCacheService.SetAddAsync(key, factory!);
        
        // Assert
        _database.Verify(database => database.SetAddAsync(
                It.Is<RedisKey>(redisKey => redisKey.Equals(key)), 
                It.IsAny<RedisValue>(), 
                It.IsAny<CommandFlags>()), 
            Times.Once());
    }
    
    [Fact]
    public async Task SetAddAsync_ShouldNotCacheObject_WhenValueIsNull()
    {
        // Arrange
        var key = _faker.Lorem.Word();
        var factory = new Func<Task<CacheLogDataCommand?>>(() => Task.Run(() => null as CacheLogDataCommand));
        
        // Act
        await _redisCacheService.SetAddAsync(key, factory!);
        
        // Assert
        _database.Verify(database => database.SetAddAsync(
                It.Is<RedisKey>(redisKey => redisKey.Equals(key)), 
                It.IsAny<RedisValue>(), 
                It.IsAny<CommandFlags>()), 
            Times.Never);
    }
    
    [Fact]
    public async Task SetAddAsync_ShouldCacheObjectInSet_WhenValueIsObject()
    {
        // Arrange
        var key = _faker.Lorem.Word();
        
        // Act
        await _redisCacheService.SetAddAsync(key, FakeDataGenerator.GenerateFakeCacheLogDataCommand());
        
        // Assert
        _database.Verify(database => database.SetAddAsync(
                It.Is<RedisKey>(redisKey => redisKey.Equals(key)), 
                It.IsAny<RedisValue>(), 
                It.IsAny<CommandFlags>()), 
            Times.Once());
    }
    
    [Fact]
    public async Task SetAddRangeAsync_ShouldCacheObjects_WhenValueIsFactoryCollection()
    {
        // Arrange
        var count = new Random().Next(10);
        var key = _faker.Lorem.Word();
        var factory = new Func<IQueryable<CacheLogDataCommand>>(() =>
            FakeDataGenerator.GenerateAsyncFakeCacheLogDataCommands(count).ToBlockingEnumerable().AsQueryable());

        // Act
        await _redisCacheService.SetAddRangeAsync(key, factory);
        
        // Assert
        _database.Verify(database => database.SetAddAsync(
                It.Is<RedisKey>(redisKey => redisKey.Equals(key)), 
                It.IsAny<RedisValue>(), 
                It.IsAny<CommandFlags>()), 
            Times.Exactly(count));
    }
    
    [Fact]
    public async Task GetSetOrAddRangeQueryableAsync_ShouldGetCachedObjects_WhenValueIsNotInCache()
    {
        // Arrange
        var count = new Random().Next(10);
        var key = _faker.Lorem.Word();
        var factory = new Func<IQueryable<CacheLogDataCommand>>(() =>
            FakeDataGenerator.GenerateAsyncFakeCacheLogDataCommands(count).ToBlockingEnumerable().AsQueryable());

        _database.Setup(database => database.SetLength(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .Returns(0);

        // Act
        await foreach(var _ in _redisCacheService.GetSetOrAddRangeAsync(key, factory)) {
        }
        
        // Assert
        _database.Verify(database => database.SetAddAsync(
                It.Is<RedisKey>(redisKey => redisKey.Equals(key)), 
                It.IsAny<RedisValue>(), 
                It.IsAny<CommandFlags>()), 
            Times.Exactly(count));
    }
}