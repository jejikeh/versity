using Application.RequestHandlers.Products.Queries.GetAllProducts;
using Application.RequestHandlers.Sessions.Commands.CreateSession;
using Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;
using Application.RequestHandlers.Sessions.Queries.GetAllSessions;
using Application.RequestHandlers.Sessions.Queries.GetSessionById;
using Application.RequestHandlers.Sessions.Queries.GetUserSessionsByUserId;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Sessions.Tests.Application;

namespace Sessions.Tests.Presentation.Controllers;

public class SessionsControllerTests
{
    private readonly Mock<ISender> _sender;
    private readonly SessionsController _sessionsController;

    public SessionsControllerTests()
    {
        _sender = new Mock<ISender>();
        _sessionsController = new SessionsController(_sender.Object);
    }

    [Fact]
    public async Task GetAllSessions_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetAllSessionsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionViewModels(new Random().Next(1, 10)));
        
        // Act
        var response = await _sessionsController.GetAllSessions(new Random().Next(1, 10), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetAllProducts_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeProducts(new Random().Next(1, 10)));
        
        // Act
        var response = await _sessionsController.GetAllProducts(new Random().Next(1, 10), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetSessionById_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetSessionByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionByIdViewModel(new Random().Next(1, 10)));
        
        // Act
        var response = await _sessionsController.GetSessionById(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetUserSessionsByUserId_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetUserSessionsByUserIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeUsersSessionsViewModel(new Random().Next(1, 10)));
        
        // Act
        var response = await _sessionsController.GetUserSessionsByUserId(Guid.NewGuid(), new Random().Next(1, 10), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetAllProductSessions_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetAllProductSessionsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionViewModels(new Random().Next(1, 10)));
        
        // Act
        var response = await _sessionsController.GetAllProductSessions(Guid.NewGuid(), new Random().Next(1, 10), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task CreateSession_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<CreateSessionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionViewModel());
        
        // Act
        var response = await _sessionsController.CreateSession(FakeDataGenerator.GenerateFakeCreateSessionCommand(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task DeleteSession_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        var response = await _sessionsController.DeleteSession(id, CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task CloseSession_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        var response = await _sessionsController.CloseSession(id, CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkResult>();
    }
}