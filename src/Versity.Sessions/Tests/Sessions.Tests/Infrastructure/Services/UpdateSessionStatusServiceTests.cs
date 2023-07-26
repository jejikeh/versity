using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Bogus;
using Domain.Models;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Sessions.Tests.Application;

namespace Sessions.Tests.Infrastructure.Services;

public class UpdateSessionStatusServiceTests
{
    private readonly Mock<ISessionsRepository> _sessionsRepository;
    private readonly Mock<ILogger<UpdateSessionStatusService>> _logger;
    private readonly Mock<INotificationSender> _notificationSender;
    private readonly UpdateSessionStatusService _updateSessionStatusService;
    
    public UpdateSessionStatusServiceTests()
    {
        _sessionsRepository = new Mock<ISessionsRepository>();
        _logger = new Mock<ILogger<UpdateSessionStatusService>>();
        _notificationSender = new Mock<INotificationSender>();
        _updateSessionStatusService = new UpdateSessionStatusService(_sessionsRepository.Object, _logger.Object, _notificationSender.Object);
    }

    [Fact]
    public void ExpireExpiredSessions_ShouldSetExpiredStatus_WhenSessionIsExpired()
    {
        // Arrange
        var count = new Random().Next(10, 100);
        var sessions = GenerateFakeExpiredOrNotExpiredSessions(count, new Random().Next(1, 10));
        var expectedCount = sessions
            .Where(x => x.Expiry < DateTime.UtcNow)
            .Count(x => x.Status != SessionStatus.Closed && x.Status != SessionStatus.Expired);
        
        _sessionsRepository.Setup(repository => repository.GetAllSessions()).Returns(sessions.AsQueryable());
        
        // Act
        _updateSessionStatusService.ExpireExpiredSessions();
        
        // Assert
        _sessionsRepository.Verify(
            repository => repository.UpdateSession(It.IsAny<Session>()), 
            Times.Exactly(expectedCount));
    }
    
    [Fact]
    public void ExpireExpiredSessions_ShouldPushNotification_WhenSessionIsExpired()
    {
        // Arrange
        var count = new Random().Next(10, 100);
        var sessions = GenerateFakeExpiredOrNotExpiredSessions(count, new Random().Next(1, 10));
        var expectedCount = sessions
            .Where(x => x.Expiry < DateTime.UtcNow)
            .Count(x => x.Status != SessionStatus.Closed && x.Status != SessionStatus.Expired);
        
        _sessionsRepository.Setup(repository => repository.GetAllSessions()).Returns(sessions.AsQueryable());
        
        // Act
        _updateSessionStatusService.ExpireExpiredSessions();
        
        // Assert
        _notificationSender.Verify(
            repository => repository.PushClosedSession(
                It.IsAny<string>(), 
                It.IsAny<UserSessionsViewModel>()), 
            Times.Exactly(expectedCount));
    }
    
    [Fact]
    public void OpenInactiveSessions_ShouldSetActiveStatus_WhenSessionIsExpired()
    {
        // Arrange
        var count = new Random().Next(10, 100);
        var sessions = GenerateFakeInactiveOrNotInactiveSessions(count, new Random().Next(1, 10));
        var expectedCount = sessions
            .Where(x => x.Start <= DateTime.Now && x.Expiry > DateTime.UtcNow)
            .Count(x => x.Status == SessionStatus.Inactive);
        
        _sessionsRepository.Setup(repository => repository.GetAllSessions()).Returns(sessions.AsQueryable());
        
        // Act
        _updateSessionStatusService.OpenInactiveSessions();
        
        // Assert
        _sessionsRepository.Verify(
            repository => repository.UpdateSession(It.IsAny<Session>()), 
            Times.Exactly(expectedCount));
    }
    
    [Fact]
    public void OpenInactiveSessions_ShouldPushNotification_WhenSessionIsOpening()
    {
        // Arrange
        var count = new Random().Next(10, 100);
        var sessions = GenerateFakeInactiveOrNotInactiveSessions(count, new Random().Next(1, 10));
        var expectedCount = sessions
            .Where(x => x.Start <= DateTime.Now && x.Expiry > DateTime.UtcNow)
            .Count(x => x.Status == SessionStatus.Inactive);
        
        _sessionsRepository.Setup(repository => repository.GetAllSessions()).Returns(sessions.AsQueryable());
        
        // Act
        _updateSessionStatusService.OpenInactiveSessions();
        
        // Assert
        _notificationSender.Verify(
            repository => repository.PushCreatedNewSession(
                It.IsAny<string>(), 
                It.IsAny<UserSessionsViewModel>()), 
            Times.Exactly(expectedCount));
    }
    
    private static List<Session> GenerateFakeExpiredOrNotExpiredSessions(int count, int logCount)
    {
        var random = new Random();
        return new Faker<Session>().CustomInstantiator(faker => new Session()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            Status = (SessionStatus)random.Next(5),
            Product = FakeDataGenerator.GenerateFakeProduct(),
            Logs = FakeDataGenerator.GenerateFakeSessionLogs(logCount),
            Expiry = random.Next(1, 10) == 9 ? faker.Date.Future() : faker.Date.Past(),
            Start = faker.Date.Past()
        }).Generate(count);
    }
    
    private static List<Session> GenerateFakeInactiveOrNotInactiveSessions(int count, int logCount)
    {
        var random = new Random();
        return new Faker<Session>().CustomInstantiator(faker => new Session()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            Status = (SessionStatus)random.Next(5),
            Product = FakeDataGenerator.GenerateFakeProduct(),
            Logs = FakeDataGenerator.GenerateFakeSessionLogs(logCount),
            Expiry = faker.Date.Between(DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(5)),
            Start = faker.Date.Between(DateTime.UtcNow.AddYears(-10), DateTime.UtcNow.AddYears(-5))
        }).Generate(count);
    }
}