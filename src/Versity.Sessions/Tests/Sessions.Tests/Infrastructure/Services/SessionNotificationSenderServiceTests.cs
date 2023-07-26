using System.Linq.Expressions;
using Application.Abstractions.Hubs;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Sessions.Tests.Application;

namespace Sessions.Tests.Infrastructure.Services;

public class SessionNotificationSenderServiceTests
{
    private readonly Mock<ILogger<SessionNotificationsSenderService>> _logger;
    private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
    private readonly SessionNotificationsSenderService _sessionNotificationsSenderService;

    public SessionNotificationSenderServiceTests()
    {
        _logger = new Mock<ILogger<SessionNotificationsSenderService>>();
        _backgroundJobClient = new Mock<IBackgroundJobClient>();
        _sessionNotificationsSenderService = new SessionNotificationsSenderService(_logger.Object, _backgroundJobClient.Object);
    }

    [Fact]
    public void PushClosedSession_ShouldInvokeBackgroundJob_WhenSessionIsClosed()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var userViewModel = FakeDataGenerator.GenerateFakeUserSessionsViewModel();
        
        // Act
        _sessionNotificationsSenderService.PushClosedSession(userId, userViewModel);
        
        // Assert
        _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(ISessionsHubHelper)), It.IsAny<IState>()), Times.Once());
    }
    
    [Fact]
    public void PushCreatedNewSession_ShouldInvokeBackgroundJob_WhenSessionIsClosed()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var userViewModel = FakeDataGenerator.GenerateFakeUserSessionsViewModel();
        
        // Act
        _sessionNotificationsSenderService.PushCreatedNewSession(userId, userViewModel);
        
        // Assert
        _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(ISessionsHubHelper)), It.IsAny<IState>()), Times.Once());
    }
}