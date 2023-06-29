using Application.Abstractions.Repositories;
using Grpc.Core;
using Infrastructure;
using MediatR;

namespace Presentation.DataServices;

public class GrpcUsersService : GrpcUsers.GrpcUsersBase
{
    private readonly IVersityUsersRepository _usersRepository;
    private readonly ISender _sender;

    public GrpcUsersService(IVersityUsersRepository usersRepository, ISender sender)
    {
        _usersRepository = usersRepository;
        _sender = sender;
    }

    public override Task<GrpcUserRoles> GetUserRoles(GetUserRolesRequest request, ServerCallContext context)
    {
        
    }
}