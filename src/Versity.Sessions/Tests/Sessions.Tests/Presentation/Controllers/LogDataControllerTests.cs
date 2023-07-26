using Application.RequestHandlers.SessionLogging.Commands.CreateLogsData;
using Application.RequestHandlers.SessionLogging.Queries.GetAllLogsData;
using Application.RequestHandlers.SessionLogging.Queries.GetLogDataById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Sessions.Tests.Application;

namespace Sessions.Tests.Presentation.Controllers;

public class LogDataControllerTests
{
    private readonly Mock<ISender> _sender;
    private readonly LogDataController _logDataController;

    public LogDataControllerTests()
    {
        _sender = new Mock<ISender>();
        _logDataController = new LogDataController(_sender.Object);
    }

    [Fact]
    public async Task GetAllLogsData_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetAllLogsDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeLogsData(new Random().Next(1, 10)));
        
        // Act
        var response = await _logDataController.GetAllLogsData(new Random().Next(1, 10), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetLogDataById_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<GetLogDataByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeLogData());
        
        // Act
        var response = await _logDataController.GetLogDataById(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task CreateLogsData_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        _sender.Setup(x => x.Send(It.IsAny<CreateLogsDataCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeLogsData(new Random().Next(1, 10)));
        
        // Act
        var response = await _logDataController.CreateLogsData(
            Guid.NewGuid(), 
            FakeDataGenerator.GenerateFakeCreateLogDataDto(new Random().Next(1, 10)), 
            CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
}