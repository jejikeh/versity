using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;
using Utils = Application.Common.Utils;

namespace Users.Tests.Application.RequestHandlers.Users;

public class GiveAdminRoleToUserTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly Mock<IAuthTokenGeneratorService> _tokenGeneratorService;
    private readonly GiveAdminRoleToUserCommandHandler _giveAdminRoleToUserCommandHandler;

    public GiveAdminRoleToUserTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _tokenGeneratorService = new Mock<IAuthTokenGeneratorService>();
        _giveAdminRoleToUserCommandHandler = new GiveAdminRoleToUserCommandHandler(_versityUsersRepository.Object, _tokenGeneratorService.Object);
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExists()
    {
        // Arrange
        var request = new GiveAdminRoleToUserCommand(new Faker().Internet.Email());
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        // Act
        var act = async () => await _giveAdminRoleToUserCommandHandler.Handle(request, default);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnString_WhenUserExists()
    {
        // Arrange
        var token = Utils.GenerateRandomString(10);
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());

        _versityUsersRepository.Setup(versityUsersRepository =>
                versityUsersRepository.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});

        _tokenGeneratorService.Setup(authTokenGeneratorService =>
                authTokenGeneratorService.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Returns(token);

        var request = new GiveAdminRoleToUserCommand(new Faker().Internet.Email());
        
        // Act
        var result =  await _giveAdminRoleToUserCommandHandler.Handle(request, default);

        // Assert
        result.Should().BeSameAs(token);
    }
}