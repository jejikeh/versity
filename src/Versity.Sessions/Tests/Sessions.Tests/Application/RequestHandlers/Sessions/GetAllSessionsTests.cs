using Application.Abstractions.Repositories;
using Application.Common;
using Application.RequestHandlers.Sessions.Queries.GetAllSessions;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Sessions;

public class GetAllSessionsTests
{
    private readonly Mock<ISessionsRepository> _sessions;

    public GetAllSessionsTests()
    {
        _sessions = new Mock<ISessionsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSessions_WhenCalled()
    {
        // Arrange
        const int entriesCount = PageFetchSettings.ItemsOnPage + 5;
        var sessionLogs = FakeDataGenerator.GenerateFakeSessions(entriesCount, 10);
        var handler = new GetAllSessionsQueryHandler(_sessions.Object);
        _sessions.Setup(repository => repository.GetAllSessions())
            .Returns(sessionLogs.AsQueryable());

        _sessions.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(sessionLogs);
        
        // Act
        await handler.Handle(new GetAllSessionsQuery(2), CancellationToken.None);
        
        // Assert
        _sessions.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<Session>>(queryable => queryable.Count() == entriesCount - PageFetchSettings.ItemsOnPage)), 
            Times.Once()); 
    }
    
    [Fact]
    public async Task Handle_ShouldReturnCorrectCountOfSession_WhenCalledWithPage()
    {
        // Arrange
        var sessionLogs = FakeDataGenerator.GenerateFakeSessions(PageFetchSettings.ItemsOnPage + 5, 10);
        var handler = new GetAllSessionsQueryHandler(_sessions.Object);
        _sessions.Setup(repository => repository.GetAllSessions())
            .Returns(sessionLogs.AsQueryable());

        _sessions.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(sessionLogs);
        
        // Act
        await handler.Handle(new GetAllSessionsQuery(1), CancellationToken.None);
        
        // Assert
        _sessions.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<Session>>(queryable => queryable.Count() == PageFetchSettings.ItemsOnPage)), 
            Times.Once()); 
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