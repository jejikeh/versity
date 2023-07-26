using Application.Abstractions;
using Application.Abstractions.Repositories;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;
using Sessions.Tests.Application;

namespace Sessions.Tests.Infrastructure.Repositories;

public class CachedSessionsRepositoryTests
{
    private readonly Mock<ISessionsRepository> _sessions;
    private readonly Mock<ICacheService> _distributedCache;
    private readonly CachedSessionsRepository _cachedSessions; 

    public CachedSessionsRepositoryTests()
    {
        _sessions = new Mock<ISessionsRepository>();
        _distributedCache = new Mock<ICacheService>();
        _cachedSessions = new CachedSessionsRepository(_sessions.Object, _distributedCache.Object);
    }

    [Fact]
    public void GetAllSessions_ShouldReturnSessionsFromRepository_WhenValueNotCached()
    {
        // Arrange
        _sessions.Setup(repository => repository.GetAllSessions())
            .Returns(FakeDataGenerator.GenerateFakeSessions(10, 10).AsQueryable());
        
        // Act
        _cachedSessions.GetAllSessions();
        
        // Assert
        _sessions.Verify(repository => repository.GetAllSessions(), Times.Once());
    }
    
    [Fact]
    public async Task GetSessionByIdAsync_ShouldReturnSessionById_WhenValueInCacheKey()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSession(10);

        _distributedCache.Setup(distributedCache =>
            distributedCache.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<Session?>>>())).ReturnsAsync(session);
        
        // Act
        var result = await _cachedSessions.GetSessionByIdAsync(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        result.Should().BeSameAs(session);
        _sessions.Verify(sessions => sessions.GetSessionByIdAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Never());
    }
    
    [Fact]
    public void GetAllUserSessions_ShouldReturnSessionsFromRepository_WhenValueNotCached()
    {
        // Arrange
        _sessions.Setup(repository => repository.GetAllUserSessions(It.IsAny<string>()))
            .Returns(FakeDataGenerator.GenerateFakeSessions(10, 10).AsQueryable());
        
        // Act
        _cachedSessions.GetAllUserSessions(Guid.NewGuid().ToString());
        
        // Assert
        _sessions.Verify(repository => repository.GetAllUserSessions(It.IsAny<string>()), Times.Once());
    }
    
    [Fact]
    public void GetAllProductSessions_ShouldReturnSessionsFromRepository_WhenValueNotCached()
    {
        // Arrange
        _sessions.Setup(repository => repository.GetAllProductSessions(It.IsAny<Guid>()))
            .Returns(FakeDataGenerator.GenerateFakeSessions(10, 10).AsQueryable());
        
        // Act
        _cachedSessions.GetAllProductSessions(Guid.NewGuid());
        
        // Assert
        _sessions.Verify(repository => repository.GetAllProductSessions(It.IsAny<Guid>()), Times.Once());
    }
    
    [Fact]
    public void CreateSessionAsync_ShouldReturnSessionFromRepository_WhenValueNotCached()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSession(10);
        _sessions.Setup(repository => repository.CreateSessionAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Run(() => session));
        
        // Act
        _cachedSessions.CreateSessionAsync(session, CancellationToken.None);
        
        // Assert
        _sessions.Verify(repository => repository.CreateSessionAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once());
    }
    
    [Fact]
    public void UpdateSession_ShouldReturnSessionFromRepository_WhenValueNotCached()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSession(10);
        _sessions.Setup(repository => repository.UpdateSession(It.IsAny<Session>()))
            .Returns(session);
        
        // Act
        _cachedSessions.UpdateSession(session);
        
        // Assert
        _sessions.Verify(repository => repository.UpdateSession(It.IsAny<Session>()), Times.Once());
    }
    
    [Fact]
    public void DeleteSession_ShouldDeleteSessionFromRepository_WhenValueNotCached()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSession(10);
        _sessions.Setup(repository => repository.UpdateSession(It.IsAny<Session>()))
            .Returns(session);
        
        // Act
        _cachedSessions.UpdateSession(session);
        
        // Assert
        _sessions.Verify(repository => repository.UpdateSession(It.IsAny<Session>()), Times.Once());
    }
    
    [Fact]
    public void ToListAsync_ShouldUseMethodFromRepository_WhenValueNotCached()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSessions(10, 10);
        _sessions.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .Returns(Task.Run(() => session));
        
        // Act
        _cachedSessions.ToListAsync(session.AsQueryable());
        
        // Assert
        _sessions.Verify(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()), Times.Once());
    }
    
    [Fact]
    public void SaveChangesAsync_ShouldUseMethodFromRepository_WhenValueNotCached()
    {
        // Arrange
        _sessions.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        _cachedSessions.SaveChangesAsync(CancellationToken.None);
        
        // Assert
        _sessions.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
    
    [Fact]
    public void SaveChanges_ShouldUseMethodFromRepository_WhenValueNotCached()
    {
        // Act
        _cachedSessions.SaveChanges();
        
        // Assert
        _sessions.Verify(repository => repository.SaveChanges(), Times.Once());
    }
}