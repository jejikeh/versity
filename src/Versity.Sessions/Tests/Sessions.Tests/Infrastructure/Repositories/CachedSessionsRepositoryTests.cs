using Application.Abstractions;
using Application.Abstractions.Repositories;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Persistence.MongoRepositories;
using Moq;
using Sessions.Tests.Application;

namespace Sessions.Tests.Infrastructure.Repositories;

public class CachedSessionsRepositoryTests
{
    private readonly Mock<ISessionsRepository> _sessionsRepository;
    private readonly Mock<ICacheService> _distributedCache;
    private readonly CachedSessionsRepository _cachedSessions; 

    public CachedSessionsRepositoryTests()
    {
        _sessionsRepository = new Mock<ISessionsRepository>();
        _distributedCache = new Mock<ICacheService>();
        _cachedSessions = new CachedSessionsRepository(_sessionsRepository.Object, _distributedCache.Object);
    }

    [Fact]
    public void GetAllSessions_ShouldReturnSessionsFromRepository_WhenValueNotCached()
    {
        // Arrange
        _sessionsRepository.Setup(repository => repository.GetSessions(
                It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(FakeDataGenerator.GenerateFakeSessions(10, 10).AsQueryable());
        
        // Act
        _cachedSessions.GetSessions(1, 2);
        
        // Assert
        _sessionsRepository.Verify(repository => repository.GetSessions(1, 2), Times.Once());
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
        _sessionsRepository.Verify(sessions => sessions.GetSessionByIdAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Never());
    }
    
    [Fact]
    public void GetAllUserSessions_ShouldReturnSessionsFromRepository_WhenValueNotCached()
    {
        // Arrange
        _sessionsRepository.Setup(repository => repository.GetAllUserSessions(
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(FakeDataGenerator.GenerateFakeSessions(10, 10).AsQueryable());
        
        // Act
        _cachedSessions.GetAllUserSessions(Guid.NewGuid().ToString(), 1, 1);
        
        // Assert
        _sessionsRepository.Verify(repository => repository.GetAllUserSessions(
            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()), 
            Times.Once());
    }
    
    [Fact]
    public void GetAllProductSessions_ShouldReturnSessionsFromRepository_WhenValueNotCached()
    {
        // Arrange
        _sessionsRepository.Setup(repository => repository.GetAllProductSessions(
                It.IsAny<Guid>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(FakeDataGenerator.GenerateFakeSessions(10, 10).AsQueryable());
        
        // Act
        _cachedSessions.GetAllProductSessions(Guid.NewGuid(), Random.Shared.Next(1,100), Random.Shared.Next(1,100));
        
        // Assert
        _sessionsRepository.Verify(repository => repository.GetAllProductSessions(
            It.IsAny<Guid>(), It.IsAny<int?>(), It.IsAny<int?>()), 
            Times.Once());
    }
    
    [Fact]
    public void CreateSessionAsync_ShouldReturnSessionFromRepository_WhenValueNotCached()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSession(10);
        _sessionsRepository.Setup(repository => repository.CreateSessionAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Run(() => session));
        
        // Act
        _cachedSessions.CreateSessionAsync(session, CancellationToken.None);
        
        // Assert
        _sessionsRepository.Verify(repository => repository.CreateSessionAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once());
    }
    
    [Fact]
    public void UpdateSession_ShouldReturnSessionFromRepository_WhenValueNotCached()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSession(10);
        _sessionsRepository.Setup(repository => repository.UpdateSession(It.IsAny<Session>()))
            .Returns(session);
        
        // Act
        _cachedSessions.UpdateSession(session);
        
        // Assert
        _sessionsRepository.Verify(repository => repository.UpdateSession(It.IsAny<Session>()), Times.Once());
    }
    
    [Fact]
    public void DeleteSession_ShouldDeleteSessionFromRepository_WhenValueNotCached()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSession(10);
        _sessionsRepository.Setup(repository => repository.UpdateSession(It.IsAny<Session>()))
            .Returns(session);
        
        // Act
        _cachedSessions.UpdateSession(session);
        
        // Assert
        _sessionsRepository.Verify(repository => repository.UpdateSession(It.IsAny<Session>()), Times.Once());
    }
    
    [Fact]
    public void SaveChangesAsync_ShouldUseMethodFromRepository_WhenValueNotCached()
    {
        // Arrange
        _sessionsRepository.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        _cachedSessions.SaveChangesAsync(CancellationToken.None);
        
        // Assert
        _sessionsRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
    
    [Fact]
    public void SaveChanges_ShouldUseMethodFromRepository_WhenValueNotCached()
    {
        // Act
        _cachedSessions.SaveChanges();
        
        // Assert
        _sessionsRepository.Verify(repository => repository.SaveChanges(), Times.Once());
    }
}