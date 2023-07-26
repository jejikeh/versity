using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Application.RequestHandlers.Sessions.Queries.GetSessionById;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Sessions;

public class GetSessionByIdTests
{
    private readonly Mock<ISessionsRepository> _sessions;

    public GetSessionByIdTests()
    {
        _sessions = new Mock<ISessionsRepository>();
    }
    
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenLogDataIsNotFound()
    {
        // Arrange
        var handler = new GetSessionByIdQueryHandler(_sessions.Object);
        _sessions.Setup(x => x.GetSessionByIdAsync(
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Session);
        
        // Act
        var act = async () => await handler.Handle(
            new GetSessionByIdQuery(Guid.NewGuid()), 
            CancellationToken.None);
        
        // Assert  
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnLog_WhenLogDataIsFound()
    {
        // Arrange
        var session = FakeDataGenerator.GenerateFakeSession(new Random().Next(10));
        var handler = new GetSessionByIdQueryHandler(_sessions.Object);
        _sessions.Setup(x => x.GetSessionByIdAsync(
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        
        // Act
        var result =  await handler.Handle(
            new GetSessionByIdQuery(Guid.NewGuid()), 
            CancellationToken.None);
        
        // Assert  
        result.Should().Be(GetSessionByIdViewModel.MapWithModel(session));
    }
}