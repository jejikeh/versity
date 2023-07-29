using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Application.Exceptions;
using Application.RequestHandlers.SessionLogging.Commands.CacheLogData;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Sessions.Tests.Application;

namespace Sessions.Tests.Infrastructure.Services;

public class BackgroundWorkersCacheServiceTests
{
    private readonly Mock<ICacheService> _cacheService;
    private readonly Mock<ISessionLogsRepository> _sessionLogsRepository;
    private readonly Mock<ILogsDataRepository> _logsDataRepository;
    private readonly BackgroundWorkersCacheService _workersCacheService;

    public BackgroundWorkersCacheServiceTests()
    {
        var logger = new Mock<ILogger<BackgroundWorkersCacheService>>();
        _cacheService = new Mock<ICacheService>();
        _sessionLogsRepository = new Mock<ISessionLogsRepository>();
        _logsDataRepository = new Mock<ILogsDataRepository>();
        
        _workersCacheService = new BackgroundWorkersCacheService(
            logger.Object,
            _cacheService.Object,
            _sessionLogsRepository.Object,
            _logsDataRepository.Object);

        _cacheService.Setup(cacheService => cacheService.GetSetAsync<CacheLogDataCommand>(CachingKeys.SessionLogs))
            .Returns(FakeDataGenerator.GenerateAsyncFakeCacheLogDataCommands(new Random().Next(1, 10)));
    }

    [Fact]
    public void PushSessionLogs_ShouldThrowNotFoundException_WhenSessionLogsIsNotFound()
    {
        // Arrange
        _sessionLogsRepository.Setup(sessionLogsRepository => sessionLogsRepository.GetSessionLogsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as SessionLogs);
        
        // Act
        var act = () => _workersCacheService.PushSessionLogs();
        
        // Assert
        act.Should().Throw<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public void PushSessionLogs_ShouldCreateLogData_WhenSessionLogsIsFound()
    {
        // Arrange
        _sessionLogsRepository.Setup(sessionLogsRepository => sessionLogsRepository.GetSessionLogsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionLogs(new Random().Next(10)));
        
        // Act
        _workersCacheService.PushSessionLogs();
        
        // Assert
        _logsDataRepository.Verify(logsDataRepository => logsDataRepository.CreateLogDataAsync(
            It.IsAny<LogData>(), 
            It.IsAny<CancellationToken>()), 
            Times.Exactly(_cacheService.Object.GetSetAsync<CacheLogDataCommand>(CachingKeys.SessionLogs).ToBlockingEnumerable().Count()));
    }
    
    [Fact]
    public void PushSessionLogs_ShouldRemoveMemberFromCache_WhenDataLogWasAdded()
    {
        // Arrange
        _sessionLogsRepository.Setup(sessionLogsRepository => sessionLogsRepository.GetSessionLogsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionLogs(new Random().Next(10)));
        
        // Act
        _workersCacheService.PushSessionLogs();
        
        // Assert
        _cacheService.Verify(cacheService => cacheService.SetRemoveMember(
                CachingKeys.SessionLogs, 
                It.IsAny<CacheLogDataCommand>()), 
            Times.Exactly(_cacheService.Object.GetSetAsync<CacheLogDataCommand>(CachingKeys.SessionLogs).ToBlockingEnumerable().Count()));
    }
    
    [Fact]
    public void PushSessionLogs_ShouldSaveChanges_WhenAllDataWasAdded()
    {
        // Arrange
        _sessionLogsRepository.Setup(sessionLogsRepository => sessionLogsRepository.GetSessionLogsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionLogs(new Random().Next(10)));
        
        // Act
        _workersCacheService.PushSessionLogs();
        
        // Assert
        _logsDataRepository.Verify(logsDataRepository => logsDataRepository.SaveChanges(), Times.Once);
        _sessionLogsRepository.Verify(sessionLogsRepository => sessionLogsRepository.SaveChanges(), Times.Once);
    }
}