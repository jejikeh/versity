using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetAllVersityUsers;

public class GetAllVersityUsersCommand : IRequest<IEnumerable<ViewVersityUserDto>>
{
    
}