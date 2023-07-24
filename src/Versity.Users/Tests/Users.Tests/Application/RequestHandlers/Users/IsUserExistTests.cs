using Application.Abstractions.Repositories;
using Application.RequestHandlers.Users.Queries.IsUserExist;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Users;

public class IsUserExistTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;

    public IsUserExistTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        _versityUsersRepository.Setup(x =>
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        var command = new IsUserExistQuery(Guid.NewGuid().ToString());
        var handler = new IsUserExistQueryHandler(_versityUsersRepository.Object);

        // Act
        var result = await handler.Handle(command, default);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        _versityUsersRepository.Setup(x =>
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var command = new IsUserExistQuery(Guid.NewGuid().ToString());
        var handler = new IsUserExistQueryHandler(_versityUsersRepository.Object);

        // Act
        var result = await handler.Handle(command, default);
        
        // Assert
        result.Should().BeFalse();
    }
}