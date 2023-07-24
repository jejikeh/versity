using Application.Abstractions;
using Application.Abstractions.Repositories;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

namespace Users.Tests.Infrastructure.Repositories;

public class CachedRefreshTokensRepositoryTests
{
    private readonly Mock<IVersityRefreshTokensRepository> _refreshTokensRepository;
    private readonly Mock<ICacheService> _distributedCache;
    
    public CachedRefreshTokensRepositoryTests()
    {
        _refreshTokensRepository = new Mock<IVersityRefreshTokensRepository>();
        _distributedCache = new Mock<ICacheService>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRefreshTokens_WhenEmptyValueInCacheKey()
    {
        // Arrange
        var refreshTokens = GenerateFakeProductsList();
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        _distributedCache.Setup(x =>
                x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<RefreshToken>?>>>()))
            .ReturnsAsync(() => null);

        _refreshTokensRepository.Setup(x => 
            x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(refreshTokens);

        // Act        
        var result = await cachedRefreshTokensRepository.GetAllAsync(CancellationToken.None);
       
        // Assert
        result.Should().BeSameAs(refreshTokens);
    }

    [Fact]
    public async Task FindUserTokenAsync_ShouldReturnRefreshToken_WhenValueInCacheKey()
    {
        // Arrange
        var refreshTokens = GenerateFakeProductsList().ToList();
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        _distributedCache.Setup(x =>
                x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<RefreshToken?>>>()))
            .ReturnsAsync(() => refreshTokens[0]);

        // Act
        var result = await cachedRefreshTokensRepository.FindUserTokenAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), CancellationToken.None);
        
        // Assert
        result.Should().BeSameAs(refreshTokens[0]);
    }
    
    [Fact]
    public async Task AddAsync_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Arrange
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        // Act
        await cachedRefreshTokensRepository.AddAsync(new RefreshToken(), CancellationToken.None);

        // Assert
        _refreshTokensRepository.Verify(x => 
            x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public void Update_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Arrange
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        // Act
        cachedRefreshTokensRepository.Update(new RefreshToken());

        // Assert
        _refreshTokensRepository.Verify(x => 
                x.Update(It.IsAny<RefreshToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task SaveChangesAsync_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Arrange
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        // Act
        await cachedRefreshTokensRepository.SaveChangesAsync(CancellationToken.None);

        // Assert
        _refreshTokensRepository.Verify(x => 
                x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    private static IEnumerable<RefreshToken> GenerateFakeProductsList()
    {
        return Enumerable.Range(0, 10).Select(_ => new RefreshToken() { Token = new Guid().ToString() }).ToList();
    }
}