using Application.RequestHandlers.SessionLogging.Queries.GetAllSessionsLogs;
using Application.RequestHandlers.SessionLogging.Queries.GetSessionLogsById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Sessions.Tests.Application;

namespace Sessions.Tests.Presentation.Controllers;

public class SessionLogsControllerTests
{
    private readonly Mock<ISender> _sender;
    private readonly SessionLogsController _sessionLogsController;

    public SessionLogsControllerTests()
    {
        _sender = new Mock<ISender>();
        _sessionLogsController = new SessionLogsController(_sender.Object);
    }

    [Fact]
    public async Task GetAllSessionLogs_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetAllSessionsLogsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionsLogs(new Random().Next(1, 10), new Random().Next(1, 10)));
        
        // Act
        var response = await _sessionLogsController.GetAllSessionLogs(new Random().Next(1, 10), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetSessionLogsById_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetSessionLogsByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeSessionLogs(new Random().Next(1, 10)));
        
        // Act
        var response = await _sessionLogsController.GetSessionLogsById(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
}