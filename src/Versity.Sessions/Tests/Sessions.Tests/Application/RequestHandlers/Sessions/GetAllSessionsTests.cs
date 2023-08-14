using Application.Abstractions.Repositories;
using Application.Common;
using Application.RequestHandlers.Sessions.Queries.GetAllSessions;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Sessions;

public class GetAllSessionsTests
{
    private readonly Mock<ISessionsRepository> _sessionsRepository;
    private readonly GetAllSessionsQueryHandler _getAllSessionsQueryHandler;

    public GetAllSessionsTests()
    {
        _sessionsRepository = new Mock<ISessionsRepository>();
        _getAllSessionsQueryHandler = new GetAllSessionsQueryHandler(_sessionsRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSessions_WhenCalled()
    {
        // Arrange
        const int entriesCount = PageFetchSettings.ItemsOnPage + 5;
        var sessionLogs = FakeDataGenerator.GenerateFakeSessions(entriesCount, 10);
        _sessionsRepository.Setup(repository => repository.GetSessions(
                It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(sessionLogs);

        _sessionsRepository.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(sessionLogs);
        
        // Act
        var result = await _getAllSessionsQueryHandler.Handle(new GetAllSessionsQuery(2), CancellationToken.None);
        
        // Assert
        result.Count().Should().Be(entriesCount);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnCorrectCountOfSession_WhenCalledWithPage()
    {
        // Arrange
        var sessionLogs = FakeDataGenerator.GenerateFakeSessions(PageFetchSettings.ItemsOnPage + 5, 10);
        _sessionsRepository.Setup(repository => repository.GetSessions(
                It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(sessionLogs);

        _sessionsRepository.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(sessionLogs);
        
        // Act
        var result = await _getAllSessionsQueryHandler.Handle(new GetAllSessionsQuery(1), CancellationToken.None);
        
        // Assert
        result.Count().Should().Be(PageFetchSettings.ItemsOnPage + 5);
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPageNumberIsLessThanOne()
    {
        // Arrange
        var validator = new GetAllSessionQueryValidator();
        var command = new GetAllSessionsQuery(-new Random().Next(0, 10));
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenPageNumberIsGreaterThanZero()
    {
        // Arrange
        var validator = new GetAllSessionQueryValidator();
        var command = new GetAllSessionsQuery(new Random().Next(1, 10));
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
}