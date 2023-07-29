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
    private readonly Mock<ISessionsRepository> _sessionsRepository;
    private readonly GetSessionByIdQueryHandler _getSessionByIdQueryHandler;

    public GetSessionByIdTests()
    {
        _sessionsRepository = new Mock<ISessionsRepository>();
        _getSessionByIdQueryHandler = new GetSessionByIdQueryHandler(_sessionsRepository.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenLogDataIsNotFound()
    {
        // Arrange
        _sessionsRepository.Setup(x => x.GetSessionByIdAsync(
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Session);
        
        // Act
        var act = async () => await _getSessionByIdQueryHandler.Handle(
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
        _sessionsRepository.Setup(x => x.GetSessionByIdAsync(
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        
        // Act
        var result =  await _getSessionByIdQueryHandler.Handle(
            new GetSessionByIdQuery(Guid.NewGuid()), 
            CancellationToken.None);
        
        // Assert  
        result.Should().Be(GetSessionByIdViewModel.MapWithModel(session));
    }
}