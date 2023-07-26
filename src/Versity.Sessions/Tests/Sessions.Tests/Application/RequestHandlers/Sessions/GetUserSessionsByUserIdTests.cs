using System.Security.Claims;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Application.Exceptions;
using Application.RequestHandlers.Sessions.Queries.GetUserSessionsByUserId;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Sessions;

public class GetUserSessionsByUserIdTests
{
    private readonly Mock<ISessionsRepository> _sessions;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly Mock<IVersityUsersDataService> _usersDataService;
    private readonly GetUserSessionsByUserIdQueryHandler _handler;

    public GetUserSessionsByUserIdTests()
    {
        _sessions = new Mock<ISessionsRepository>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _usersDataService = new Mock<IVersityUsersDataService>();
        _handler = new GetUserSessionsByUserIdQueryHandler(
            _sessions.Object, 
            _httpContextAccessor.Object, 
            _usersDataService.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenUserClaimsIsInvalid()
    {
        // Arrange
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims)
            .Returns(() => new List<Claim>());
        
        // Act
        var act = async () => await _handler.Handle(new GetUserSessionsByUserIdQuery(Guid.NewGuid().ToString(), 2), default);
        
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Fact]
    public async Task Handle_ShouldThrowExceptionWithStatusCode_WhenUserIdFromClaimNotTheSame()
    {
        // Arrange
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims)
            .Returns(new[] {GenerateFakeUserClaims()} );

        // Act
        var act = async () => await _handler.Handle(new GetUserSessionsByUserIdQuery(Guid.NewGuid().ToString(), 2), default);
        
        // Assert
        await act.Should().ThrowAsync<ExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSessions_WhenUserIdAdmin()
    {
        // Arrange
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims)
            .Returns(new [] { GenerateFakeUserClaims() });
        
        _usersDataService.Setup(x => x.GetUserRolesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<string>() { "Admin"});

        var fakeSessions = FakeDataGenerator.GenerateFakeSessions(15, 5);
        _sessions.Setup(x => x.GetAllUserSessions(It.IsAny<string>()))
            .Returns(fakeSessions.AsQueryable());

        _sessions.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(fakeSessions);
        
        // Act
        await _handler.Handle(new GetUserSessionsByUserIdQuery(Guid.NewGuid().ToString(), 2), default);
        
        // Assert
        _sessions.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<Session>>(queryable => queryable.Count() == 5)), 
            Times.Once());
    }
    
    [Fact]
    public async Task Handle_ShouldReturnProperCountOfSessions_WhenPageNumberIsThanCountOfEntries()
    {
        // Arrange
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims)
            .Returns(new [] { GenerateFakeUserClaims() });
        
        _usersDataService.Setup(x => x.GetUserRolesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<string>() { "Admin"});

        var fakeSessions = FakeDataGenerator.GenerateFakeSessions(15, 5);
        _sessions.Setup(x => x.GetAllUserSessions(It.IsAny<string>()))
            .Returns(fakeSessions.AsQueryable());

        _sessions.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(fakeSessions);
        
        // Act
        await _handler.Handle(new GetUserSessionsByUserIdQuery(Guid.NewGuid().ToString(), 1), default);
        
        // Assert
        _sessions.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<Session>>(queryable => queryable.Count() == PageFetchSettings.ItemsOnPage)), 
            Times.Once());
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSessions_WhenUserIdFromClaimIsTheSame()
    {
        // Arrange
        var claimId = GenerateFakeUserClaims();
        _httpContextAccessor.Setup(x => x.HttpContext.User.Claims)
            .Returns(new [] { claimId });

        var fakeSessions = FakeDataGenerator.GenerateFakeSessions(15, 5);
        _sessions.Setup(x => x.GetAllUserSessions(It.IsAny<string>()))
            .Returns(fakeSessions.AsQueryable());
        
        _sessions.Setup(repository => repository.ToListAsync(It.IsAny<IQueryable<Session>>()))
            .ReturnsAsync(fakeSessions);
        
        // Act
        await _handler.Handle(new GetUserSessionsByUserIdQuery(claimId.Value, 2), default);
        
        // Assert
        _sessions.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<Session>>(queryable => queryable.Count() == 5)), 
            Times.Once());
    }

    private static Claim GenerateFakeUserClaims()
    {
        return new Faker<Claim>().CustomInstantiator(faker => 
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()))
            .Generate();
    }
}   