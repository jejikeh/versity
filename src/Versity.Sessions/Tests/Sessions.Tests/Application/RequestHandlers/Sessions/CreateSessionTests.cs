using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Application.RequestHandlers.Sessions.Commands.CreateSession;
using Bogus;
using Domain.Models;
using Domain.Models.SessionLogging;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Sessions;

public class CreateSessionTests
{
    private readonly Mock<ISessionsRepository> _sessionsRepository;
    private readonly Mock<IVersityUsersDataService> _usersDataService;
    private readonly Mock<IProductsRepository> _productsRepository;
    private readonly Mock<ISessionLogsRepository> _sessionLogsRepository;
    private readonly Mock<INotificationSender> _notificationSender;
    private readonly CreateSessionCommandHandler _createSessionCommandHandler;

    public CreateSessionTests()
    {
        _sessionsRepository = new Mock<ISessionsRepository>();
        _usersDataService = new Mock<IVersityUsersDataService>();
        _productsRepository = new Mock<IProductsRepository>();
        _sessionLogsRepository = new Mock<ISessionLogsRepository>();
        _notificationSender = new Mock<INotificationSender>();
        
        _createSessionCommandHandler = new CreateSessionCommandHandler(_sessionsRepository.Object, _usersDataService.Object, _productsRepository.Object, _sessionLogsRepository.Object, _notificationSender.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserIsNotFound()
    {
        // Arrange
        _usersDataService.Setup(versityUsersDataService => versityUsersDataService.IsUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        // Act
        var act = async () => await _createSessionCommandHandler.Handle(FakeDataGenerator.GenerateFakeCreateSessionCommand(), CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductIsNotFound()
    {
        // Arrange
        _usersDataService.Setup(versityUsersDataService => versityUsersDataService.IsUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _productsRepository.Setup(productsRepository => productsRepository.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Product);
        
        // Act
        var act = async () => await _createSessionCommandHandler.Handle(FakeDataGenerator.GenerateFakeCreateSessionCommand(), CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }

    [Fact]
    public async Task Handle_ShouldSetIdInSessionLogs_WhenUserAndProductIsFound()
    {
        // Arrange
        _usersDataService.Setup(versityUsersDataService => versityUsersDataService.IsUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _productsRepository.Setup(productsRepository => productsRepository.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeProduct());

        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.CreateSessionAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        var result = await _createSessionCommandHandler.Handle(FakeDataGenerator.GenerateFakeCreateSessionCommand(), CancellationToken.None);
        
        // Assert
        _sessionLogsRepository.Verify(sessionLogsRepository => sessionLogsRepository.CreateSessionLogsAsync(
            It.Is<SessionLogs>(logs => logs.SessionId == result.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldSaveChanges_WhenSessionIsCreated()
    {
        // Arrange
        _usersDataService.Setup(versityUsersDataService => versityUsersDataService.IsUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _productsRepository.Setup(productsRepository => productsRepository.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeProduct());

        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.CreateSessionAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        var result = await _createSessionCommandHandler.Handle(FakeDataGenerator.GenerateFakeCreateSessionCommand(), CancellationToken.None);
        
        // Assert
        _sessionLogsRepository.Verify(sessionLogsRepository => sessionLogsRepository.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
        
        _sessionsRepository.Verify(sessionsRepository => sessionsRepository.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldSendNotification_WhenChangesSaved()
    {
        // Arrange
        _usersDataService.Setup(versityUsersDataService => versityUsersDataService.IsUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        _productsRepository.Setup(productsRepository => productsRepository.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeProduct());

        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.CreateSessionAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSession(new Random().Next(10)));
        
        // Act
        var result = await _createSessionCommandHandler.Handle(FakeDataGenerator.GenerateFakeCreateSessionCommand(), CancellationToken.None);
        
        // Assert
        _notificationSender.Verify(notificationSender => notificationSender.PushCreatedNewSession(
                It.IsAny<string>(), It.Is<UserSessionsViewModel>(
                    model => model.Status == SessionStatus.Inactive)), 
            Times.Once);
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationFailure_WhenCommandIsNotValid()
    {
        // Arrange
        var command = GenerateFakeInvalidCreateSessionCommand();
        var validator = new CreateSessionCommandValidator();
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenCommandIsValid()
    {
        // Arrange
        var command = FakeDataGenerator.GenerateFakeCreateSessionCommand();
        var validator = new CreateSessionCommandValidator();
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    private static CreateSessionCommand GenerateFakeInvalidCreateSessionCommand()
    {
        var random = new Random();
        return new Faker<CreateSessionCommand>().CustomInstantiator(faker => new CreateSessionCommand(
            random.Next(1) == 0 ? Guid.NewGuid().ToString() : string.Empty,
            random.Next(1) == 0 ? Guid.NewGuid() : default,
            random.Next(1) == 0 ? faker.Date.Future() : faker.Date.Past(),
            faker.Date.Past()
        )).Generate();
    }
}