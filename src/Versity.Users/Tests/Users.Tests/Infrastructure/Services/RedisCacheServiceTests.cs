using System.Text.Json;
using FluentAssertions;
using Infrastructure.Services;
using Moq;
using StackExchange.Redis;

namespace Users.Tests.Infrastructure.Services;

public class RedisCacheServiceTests
{
    private readonly Mock<IDatabase> _database;
    private readonly RedisCacheService _redisCacheService;

    public RedisCacheServiceTests()
    {
        _database = new Mock<IDatabase>();
        
        var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
        connectionMultiplexer.Setup(
            x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_database.Object);
        
        _redisCacheService = new RedisCacheService(connectionMultiplexer.Object);
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldReturnValue_WhenValueInCacheKey()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var factory = new Func<Task<string?>>(() => Task.Run(() => key));
        
        _database.Setup(x => 
            x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(JsonSerializer.Serialize(key));
        
        // Act
        var result = await _redisCacheService.GetOrCreateAsync(key, factory);
        
        // Assert
        result.Should().Be(key);
    }
    
    [Fact]
    public async Task GetOrCreateAsync_ShouldCacheAndReturnValue_WhenValueNotInCacheKey()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var factory = new Mock<Func<Task<string?>>>();
        factory.Setup(x=>x.Invoke()).ReturnsAsync(key);
       
        _database.Setup(x => 
                x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(string.Empty);
        
        // Act
        var result = await _redisCacheService.GetOrCreateAsync(key, factory.Object);
        
        // Assert
        result.Should().Be(key);
    }
    
    [Fact]
    public async Task GetOrCreateAsync_ShouldReturnNull_WhenValueNotInCacheKeyAndFactoryIsNull()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var factory = new Func<Task<string?>>(() => Task.Run(() => null as string));
        
        _database.Setup(x => 
                x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(string.Empty);
        
        // Act
        var result = await _redisCacheService.GetOrCreateAsync(key, factory);
        
        // Assert
        result.Should().BeNull();
    }
}