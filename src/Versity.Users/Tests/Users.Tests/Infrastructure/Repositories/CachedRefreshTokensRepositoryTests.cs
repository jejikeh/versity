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
    private readonly CachedRefreshTokensRepository _cachedRefreshTokensRepository;
    
    public CachedRefreshTokensRepositoryTests()
    {
        _refreshTokensRepository = new Mock<IVersityRefreshTokensRepository>();
        _distributedCache = new Mock<ICacheService>();
        _cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRefreshTokens_WhenEmptyValueInCacheKey()
    {
        // Arrange
        var refreshTokens = GenerateFakeRefreshTokenList();

        _distributedCache.Setup(cacheService =>
                cacheService.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<RefreshToken>?>>>()))
            .ReturnsAsync(() => null);

        _refreshTokensRepository.Setup(x => 
            x.GetAllAsync(
                It.IsAny<int?>(), 
                It.IsAny<int?>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync(refreshTokens);

        // Act        
        var result = await _cachedRefreshTokensRepository.GetAllAsync(1, 1, CancellationToken.None);
       
        // Assert
        result.Should().BeSameAs(refreshTokens);
    }

    [Fact]
    public async Task FindUserTokenAsync_ShouldReturnRefreshToken_WhenValueInCacheKey()
    {
        // Arrange
        var refreshTokens = GenerateFakeRefreshTokenList().ToList();

        _distributedCache.Setup(cacheService =>
                cacheService.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<RefreshToken?>>>()))
            .ReturnsAsync(() => refreshTokens[0]);

        // Act
        var result = await _cachedRefreshTokensRepository.FindUserTokenAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), CancellationToken.None);
        
        // Assert
        result.Should().BeSameAs(refreshTokens[0]);
    }
    
    [Fact]
    public async Task AddAsync_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Act
        await _cachedRefreshTokensRepository.AddAsync(new RefreshToken(), CancellationToken.None);

        // Assert
        _refreshTokensRepository.Verify(versityRefreshTokensRepository => 
            versityRefreshTokensRepository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public void Update_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Act
        _cachedRefreshTokensRepository.Update(new RefreshToken());

        // Assert
        _refreshTokensRepository.Verify(versityRefreshTokensRepository => 
                versityRefreshTokensRepository.Update(It.IsAny<RefreshToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task SaveChangesAsync_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Act
        await _cachedRefreshTokensRepository.SaveChangesAsync(CancellationToken.None);

        // Assert
        _refreshTokensRepository.Verify(versityRefreshTokensRepository => 
                versityRefreshTokensRepository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    private static IEnumerable<RefreshToken> GenerateFakeRefreshTokenList()
    {
        return Enumerable.Range(0, 10).Select(_ => new RefreshToken() { Token = new Guid().ToString() }).ToList();
    }
}