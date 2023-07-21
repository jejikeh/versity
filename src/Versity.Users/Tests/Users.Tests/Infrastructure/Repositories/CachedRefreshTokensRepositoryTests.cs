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
        var refreshTokens = GenerateFakeProductsList();
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        _distributedCache.Setup(x =>
                x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<RefreshToken>?>>>()))
            .ReturnsAsync(() => null);

        _refreshTokensRepository.Setup(x => 
            x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(refreshTokens);
        
        var result = await cachedRefreshTokensRepository.GetAllAsync(CancellationToken.None);
        
        result.Should().BeSameAs(refreshTokens);
    }

    [Fact]
    public async Task FindUserTokenAsync_ShouldReturnRefreshToken_WhenValueInCacheKey()
    {
        var refreshTokens = GenerateFakeProductsList().ToList();
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        _distributedCache.Setup(x =>
                x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<RefreshToken?>>>()))
            .ReturnsAsync(() => refreshTokens[0]);

        var result = await cachedRefreshTokensRepository.FindUserTokenAsync("testUser", "testToken", CancellationToken.None);
        
        result.Should().BeSameAs(refreshTokens[0]);
    }
    
    [Fact]
    public async Task AddAsync_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        await cachedRefreshTokensRepository.AddAsync(new RefreshToken(), CancellationToken.None);

        _refreshTokensRepository.Verify(x => 
            x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public void Update_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        cachedRefreshTokensRepository.Update(new RefreshToken());

        _refreshTokensRepository.Verify(x => 
                x.Update(It.IsAny<RefreshToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task SaveChangesAsync_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        var cachedRefreshTokensRepository = new CachedRefreshTokensRepository(_refreshTokensRepository.Object, _distributedCache.Object);

        await cachedRefreshTokensRepository.SaveChangesAsync(CancellationToken.None);

        _refreshTokensRepository.Verify(x => 
                x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    private static IEnumerable<RefreshToken> GenerateFakeProductsList()
    {
        return Enumerable.Range(0, 10).Select(_ => new RefreshToken() { Token = "testToken" }).ToList();
    }
}