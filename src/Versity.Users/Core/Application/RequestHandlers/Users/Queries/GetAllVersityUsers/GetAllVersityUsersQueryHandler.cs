using Application.Abstractions.Repositories;
using Application.Common;
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
        var usersQuery = _versityUsersRepository
            .GetAllUsers()
            .OrderBy(x => x.UserName)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);

        var users = await _versityUsersRepository.ToListAsync(usersQuery);
        
        var viewVersityUserDtos = new List<ViewVersityUserDto>();
        foreach (var user in users) 
        {
            viewVersityUserDtos.Add(ViewVersityUserDto.MapFromModel(user, await _versityUsersRepository.GetRolesAsync(user)));
        }
        
        return viewVersityUserDtos;
    }
}