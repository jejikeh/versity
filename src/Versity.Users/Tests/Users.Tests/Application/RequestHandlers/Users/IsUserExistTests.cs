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
        _versityUsersRepository.Setup(x =>
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());
        
        var command = new IsUserExistQuery("f2c79b68-285b-4015-8fd0-b4b806d6b6f0");
        var handler = new IsUserExistQueryHandler(_versityUsersRepository.Object);

        var result = await handler.Handle(command, default);
        
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        _versityUsersRepository.Setup(x =>
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var command = new IsUserExistQuery("f2c79b68-285b-4015-8fd0-b4b806d6b6f0");
        var handler = new IsUserExistQueryHandler(_versityUsersRepository.Object);

        var result = await handler.Handle(command, default);
        
        result.Should().BeFalse();
    }
}