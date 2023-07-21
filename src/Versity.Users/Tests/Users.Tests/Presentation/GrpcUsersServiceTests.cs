using Application.RequestHandlers.Users.Queries.GetVersityUserRoles;
using Application.RequestHandlers.Users.Queries.IsUserExist;
using FluentAssertions;
using Grpc.Core;
using MediatR;
using Moq;
using Presentation;
using Presentation.Services;

namespace Users.Tests.Presentation;

public class GrpcUsersServiceTests
{
    private readonly Mock<ISender> _sender;
    private readonly Mock<ServerCallContext> _serverCallContext;

    public GrpcUsersServiceTests()
    {
        _sender = new Mock<ISender>();
        _serverCallContext = new Mock<ServerCallContext>();
    }

    [Fact]
    public async Task IsUserExist_ShouldReturnTrue_WhenUserExist()
    {
        _sender.Setup(x => 
                x.Send(It.IsAny<IsUserExistQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var request = new GrpcIsUserExistRequest(){ UserId = "test" };
        var service = new GrpcUsersService(_sender.Object);
        
        var response = await service.IsUserExist(request, _serverCallContext.Object);
        
        response.Should().Be(new GrpcIsUserExistResponse(){ Exist = true });
    }
    
    [Fact]
    public async Task IsUserExist_ShouldReturnFalse_WhenUserNotExist()
    {
        _sender.Setup(x => 
                x.Send(It.IsAny<IsUserExistQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var request = new GrpcIsUserExistRequest(){ UserId = "test" };
        var service = new GrpcUsersService(_sender.Object);
        
        var response = await service.IsUserExist(request, _serverCallContext.Object);
        
        response.Should().Be(new GrpcIsUserExistResponse(){ Exist = false });
    }
    
    [Fact]
    public async Task GetUserRoles_ShouldReturnRoles_WhenUserExist()
    {
        _sender.Setup(x => 
                x.Send(It.IsAny<GetVersityUserRolesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "role1", "role2" });
        var request = new GetUserRolesRequest() { UserId = "test" };
        var service = new GrpcUsersService(_sender.Object);
        
        var response = await service.GetUserRoles(request, _serverCallContext.Object);
        
        response.Should().Be(new GrpcUserRoles(){ Roles = { "role1", "role2" }});
    }
}