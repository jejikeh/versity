using Application.Abstractions.Repositories;
using MediatR;

namespace Application.RequestHandlers.Users.Queries.IsUserExist;

public class IsUserExistQueryHandler : IRequestHandler<IsUserExistQuery, bool>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public IsUserExistQueryHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<bool> Handle(IsUserExistQuery request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(request.UserId);
        
        return versityUser is not null;
    }
}