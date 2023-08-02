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
    private readonly GetVersityUserRolesQueryHandler _getVersityUserRolesQueryHandler;
    
    public GetVersityUserRolesTests()
    {
        _versityUsersRepository = new Mock<IVersityUsersRepository>();
        _getVersityUserRolesQueryHandler = new GetVersityUserRolesQueryHandler(_versityUsersRepository.Object);
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenUserDoesNotExists()
    {
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(null as VersityUser);
        
        var request = new GetVersityUserRolesQuery(new Faker().Internet.Email());

        var act = async () => await _getVersityUserRolesQueryHandler.Handle(request, default);

        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldReturnRoles_WhenUserExists()
    {
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new VersityUser{ Id = Guid.NewGuid().ToString() });
        
        _versityUsersRepository.Setup(versityUsersRepository => 
                versityUsersRepository.GetUserRolesAsync(It.IsAny<VersityUser>()))
            .ReturnsAsync(new []{VersityRole.Admin.ToString()});
        
        var request = new GetVersityUserRolesQuery(new Faker().Internet.Email());

        var result = await _getVersityUserRolesQueryHandler.Handle(request, default);

        result.Should().BeEquivalentTo(new []{"Admin"});
    }
}