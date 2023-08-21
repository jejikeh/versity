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
    private readonly CloseSessionCommandHandler _closeSessionCommandHandler;

    public CloseSessionTests()
    {
        _sessionsRepository = new Mock<ISessionsRepository>();
        _notificationSender = new Mock<INotificationSender>();
        _closeSessionCommandHandler = new CloseSessionCommandHandler(_sessionsRepository.Object, _notificationSender.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenSessionIsNotFound()
    {
        // Arrange
        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.GetSessionByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Session);
        
        // Act
        var act = async () => await _closeSessionCommandHandler.Handle(new CloseSessionCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldCloseSession_WhenSessionIsFound()
    {
        // Arrange
        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.GetSessionByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        await _closeSessionCommandHandler.Handle(new CloseSessionCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        _sessionsRepository.Verify(sessionsRepository => sessionsRepository.UpdateSessionAsync(
                It.IsAny<Session>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldSaveChanges_WhenSessionIsModified()
    {
        // Arrange
        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.GetSessionByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        await _closeSessionCommandHandler.Handle(new CloseSessionCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        _sessionsRepository.Verify(sessionsRepository => sessionsRepository.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldPushNotification_WhenSessionModificationsSaved()
    {
        // Arrange
        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.GetSessionByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        await _closeSessionCommandHandler.Handle(new CloseSessionCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        _notificationSender.Verify(notificationSender => notificationSender.PushClosedSession(
            It.IsAny<string>(), It.Is<UserSessionsViewModel>(
                model => model.Status == SessionStatus.Closed)), 
            Times.Once);
    }
}