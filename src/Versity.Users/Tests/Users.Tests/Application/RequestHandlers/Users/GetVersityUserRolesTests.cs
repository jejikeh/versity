using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Users.Queries.GetVersityUserRoles;
using Bogus;
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
        
        var request = new GetVersityUserRolesQuery(new Faker().Internet.Email());
        var handler = new GetVersityUserRolesQueryHandler(_versityUsersRepository.Object);

        var act = async () => await handler.Handle(request, default);

        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnRoles_WhenUserExists()
    {
        _versityUsersRepository.Setup(x => 
                x.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser{ Id = Guid.NewGuid().ToString() });
        
        _versityUsersRepository.Setup(x => 
                x.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});
        
        var request = new GetVersityUserRolesQuery(new Faker().Internet.Email());
        var handler = new GetVersityUserRolesQueryHandler(_versityUsersRepository.Object);

        var result = await handler.Handle(request, default);

        result.Should().BeEquivalentTo(new []{"Admin"});
    }
}