using Application.Abstractions.Repositories;
using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetAllVersityUsers;

public class GetAllVersityUsersQueryHandler : IRequestHandler<GetAllVersityUsersQuery, IEnumerable<ViewVersityUserDto>>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public GetAllVersityUsersQueryHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<IEnumerable<ViewVersityUserDto>> Handle(GetAllVersityUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _versityUsersRepository
            .GetAllUsers()
            .OrderBy(x => x.UserName)
            .Skip(10 * (request.Page - 1))
            .Take(10)
            .ToList();
        
        var viewVersityUserDtos = new List<ViewVersityUserDto>();
        foreach (var user in users) 
        {
            viewVersityUserDtos.Add(ViewVersityUserDto.MapFromModel(user, await _versityUsersRepository.GetRolesAsync(user)));
        }
        
        return viewVersityUserDtos;
    }
}