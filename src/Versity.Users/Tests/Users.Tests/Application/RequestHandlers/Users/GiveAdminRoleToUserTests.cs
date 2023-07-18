using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Users;

public class GiveAdminRoleToUserTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;
    private readonly Mock<IAuthTokenGeneratorService> _tokenGeneratorService;

    public GiveAdminRoleToUserTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _tokenGeneratorService = new Mock<IAuthTokenGeneratorService>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExists()
    {
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var request = new GiveAdminRoleToUserCommand("hello@gmail.com");
        var handler = new GiveAdminRoleToUserCommandHandler(
            _versityUsersRepository.Object,
            _tokenGeneratorService.Object
        );
        
        var act = async () => await handler.Handle(request, default);
        
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnString_WhenUserExists()
    {
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser());

        _versityUsersRepository.Setup(x =>
                x.GetRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});

        _tokenGeneratorService.Setup(x =>
                x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Returns("token");

        var request = new GiveAdminRoleToUserCommand("hello@gmail.com");
        var handler = new GiveAdminRoleToUserCommandHandler(
            _versityUsersRepository.Object,
            _tokenGeneratorService.Object
        );
        
        var result =  await handler.Handle(request, default);

        result.Should().BeSameAs("token");
    }
}