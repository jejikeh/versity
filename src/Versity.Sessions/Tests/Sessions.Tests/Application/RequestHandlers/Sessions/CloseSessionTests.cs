using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Application.RequestHandlers.Sessions.Commands.CloseSession;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Sessions;

public class CloseSessionTests
{
    private readonly Mock<ISessionsRepository> _sessionsRepository;
    private readonly Mock<INotificationSender> _notificationSender;

    public CloseSessionTests()
    {
        _sessionsRepository = new Mock<ISessionsRepository>();
        _notificationSender = new Mock<INotificationSender>();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenSessionIsNotFound()
    {
        // Arrange
        var handler = new CloseSessionCommandHandler(_sessionsRepository.Object, _notificationSender.Object);
        _sessionsRepository.Setup(x => x.GetSessionByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Session);
        
        // Act
        var act = async () => await handler.Handle(new CloseSessionCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldCloseSession_WhenSessionIsFound()
    {
        // Arrange
        var handler = new CloseSessionCommandHandler(_sessionsRepository.Object, _notificationSender.Object);
        _sessionsRepository.Setup(x => x.GetSessionByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        await handler.Handle(new CloseSessionCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        _sessionsRepository.Verify(x => x.UpdateSession(
                It.Is<Session>(session => session.Status == SessionStatus.Closed)), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldSaveChanges_WhenSessionIsModified()
    {
        // Arrange
        var handler = new CloseSessionCommandHandler(_sessionsRepository.Object, _notificationSender.Object);
        _sessionsRepository.Setup(x => x.GetSessionByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        await handler.Handle(new CloseSessionCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        _sessionsRepository.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldPushNotification_WhenSessionModificationsSaved()
    {
        // Arrange
        var handler = new CloseSessionCommandHandler(_sessionsRepository.Object, _notificationSender.Object);
        _sessionsRepository.Setup(x => x.GetSessionByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        await handler.Handle(new CloseSessionCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        _notificationSender.Verify(x => x.PushClosedSession(
            It.IsAny<string>(), It.Is<UserSessionsViewModel>(
                model => model.Status == SessionStatus.Closed)), 
            Times.Once);
    }
}