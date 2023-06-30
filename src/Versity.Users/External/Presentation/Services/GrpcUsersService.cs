using Application.Exceptions;
using Application.RequestHandlers.Users.Queries.GetVersityUserRoles;
using Application.RequestHandlers.Users.Queries.IsUserExist;
using Grpc.Core;
using Infrastructure;
using MediatR;

namespace Presentation.Services;

public class GrpcUsersService : GrpcUsers.GrpcUsersBase
{
    private readonly ISender _sender;

    public GrpcUsersService(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<GrpcIsUserExistResponse> IsUserExist(GrpcIsUserExistRequest request, ServerCallContext context)
    {
        var response = new GrpcIsUserExistResponse();
        var command = new IsUserExistQuery(request.UserId);
        try
        {
            response.Exist = await _sender.Send(command);
        }
        catch (NotFoundExceptionWithStatusCode ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        
        return response;
    }

    public override async Task<GrpcUserRoles> GetUserRoles(GetUserRolesRequest request, ServerCallContext context)
    {
        var response = new GrpcUserRoles();
        var command = new GetVersityUserRolesQuery(request.UserId);
        try
        {
            var roles = await _sender.Send(command);
            foreach (var role in roles)
            {
                response.Roles.Add(role);
            }
        }
        catch (NotFoundExceptionWithStatusCode ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        
        return response;
    }
}