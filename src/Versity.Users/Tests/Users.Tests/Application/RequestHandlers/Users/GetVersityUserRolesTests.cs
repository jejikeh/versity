using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Users.Queries.GetVersityUserRoles;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Users.Tests.Application.RequestHandlers.Users;

public class GetVersityUserRolesTests
{
    private readonly Mock<IVersityUsersRepository> _versityUsersRepository;

    public GetVersityUserRolesTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExists()
    {
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var request = new GetVersityUserRolesQuery("hello@gmail.com");
        var handler = new GetVersityUserRolesQueryHandler(_versityUsersRepository.Object);

        var act = async () => await handler.Handle(request, default);

        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnRoles_WhenUserExists()
    {
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser{ Id = "dd44e461-7217-41ab-8a41-f230381e0ed8" });
        
        _versityUsersRepository.Setup(x => 
                x.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});
        
        var request = new GetVersityUserRolesQuery("hello@gmail.com");
        var handler = new GetVersityUserRolesQueryHandler(_versityUsersRepository.Object);

        var result = await handler.Handle(request, default);

        result.Should().BeEquivalentTo(new []{"Admin"});
    }
}