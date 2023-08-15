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
    private readonly Mock<ISessionsRepository> _sessionsRepository;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly Mock<IVersityUsersDataService> _usersDataService;
    private readonly GetUserSessionsByUserIdQueryHandler _getUserSessionsByUserIdQueryHandler;

    public GetUserSessionsByUserIdTests()
    {
        _sessionsRepository = new Mock<ISessionsRepository>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _usersDataService = new Mock<IVersityUsersDataService>();
        _getUserSessionsByUserIdQueryHandler = new GetUserSessionsByUserIdQueryHandler(
            _sessionsRepository.Object, 
            _httpContextAccessor.Object, 
            _usersDataService.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenUserClaimsIsInvalid()
    {
        // Arrange
        _httpContextAccessor.Setup(httpContextAccessor => httpContextAccessor.HttpContext.User.Claims).Returns(() => new List<Claim>());
        
        // Act
        var act = async () => await _getUserSessionsByUserIdQueryHandler.Handle(new GetUserSessionsByUserIdQuery(Guid.NewGuid().ToString(), 2), default);
        
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Fact]
    public async Task Handle_ShouldThrowExceptionWithStatusCode_WhenUserIdFromClaimNotTheSame()
    {
        // Arrange
        _httpContextAccessor.Setup(httpContextAccessor => httpContextAccessor.HttpContext.User.Claims).Returns(new[] {GenerateFakeUserClaims()} );

        // Act
        var act = async () => await _getUserSessionsByUserIdQueryHandler.Handle(new GetUserSessionsByUserIdQuery(Guid.NewGuid().ToString(), 2), default);
        
        // Assert
        await act.Should().ThrowAsync<ExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSessions_WhenUserIdAdmin()
    {
        // Arrange
        _httpContextAccessor.Setup(httpContextAccessor => httpContextAccessor.HttpContext.User.Claims).Returns(new [] { GenerateFakeUserClaims() });
        
        _usersDataService.Setup(versityUsersDataService => versityUsersDataService.GetUserRolesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<string>() { "Admin"});

        var fakeSessions = FakeDataGenerator.GenerateFakeSessions(15, 5);
        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.GetAllUserSessions(
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(fakeSessions);

        // Act
        var result = await _getUserSessionsByUserIdQueryHandler.Handle(new GetUserSessionsByUserIdQuery(Guid.NewGuid().ToString(), 2), default);
        
        // Assert
        result.Count().Should().Be(fakeSessions.Count());
    }
    
    [Fact]
    public async Task Handle_ShouldReturnProperCountOfSessions_WhenPageNumberIsThanCountOfEntries()
    {
        // Arrange
        _httpContextAccessor.Setup(httpContextAccessor => httpContextAccessor.HttpContext.User.Claims).Returns(new [] { GenerateFakeUserClaims() });
        
        _usersDataService.Setup(versityUsersDataService => versityUsersDataService.GetUserRolesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<string>() { "Admin"});

        var fakeSessions = FakeDataGenerator.GenerateFakeSessions(15, 5);
        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.GetAllUserSessions(
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(fakeSessions);

        // Act
        var result = await _getUserSessionsByUserIdQueryHandler.Handle(new GetUserSessionsByUserIdQuery(Guid.NewGuid().ToString(), 1), default);
        
        // Assert
        result.Count().Should().Be(fakeSessions.Count());
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSessions_WhenUserIdFromClaimIsTheSame()
    {
        // Arrange
        var claimId = GenerateFakeUserClaims();
        _httpContextAccessor.Setup(httpContextAccessor => httpContextAccessor.HttpContext.User.Claims).Returns(new [] { claimId });

        var fakeSessions = FakeDataGenerator.GenerateFakeSessions(15, 5);
        _sessionsRepository.Setup(sessionsRepository => sessionsRepository.GetAllUserSessions(
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(fakeSessions);
        
        // Act
        var result = await _getUserSessionsByUserIdQueryHandler.Handle(new GetUserSessionsByUserIdQuery(claimId.Value, 2), default);
        
        // Assert
        result.Count().Should().Be(fakeSessions.Count());
    }

    private static Claim GenerateFakeUserClaims()
    {
        return new Faker<Claim>().CustomInstantiator(faker => 
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()))
            .Generate();
    }
}   