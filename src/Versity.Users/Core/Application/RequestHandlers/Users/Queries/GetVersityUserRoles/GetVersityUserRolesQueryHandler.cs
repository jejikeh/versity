using Application.Abstractions.Repositories;
using Application.Exceptions;
using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetVersityUserRoles;

public class GetVersityUserRolesCommandHandler : IRequestHandler<GetVersityUserRolesQuery, IEnumerable<string>>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public GetVersityUserRolesCommandHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<IEnumerable<string>> Handle(GetVersityUserRolesQuery request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(request.UserId);
        if (versityUser is null) 
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }
        
        return await _versityUsersRepository.GetUserRolesAsync(versityUser);
    }
}