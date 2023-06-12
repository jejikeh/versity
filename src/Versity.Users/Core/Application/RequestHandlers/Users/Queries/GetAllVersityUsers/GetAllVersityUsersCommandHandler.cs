using Application.Abstractions.Repositories;
using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetAllVersityUsers;

public class GetAllVersityUsersCommandHandler 
    : IRequestHandler<GetAllVersityUsersCommand, IEnumerable<ViewVersityUserDto>>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public GetAllVersityUsersCommandHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<IEnumerable<ViewVersityUserDto>> Handle(GetAllVersityUsersCommand request, CancellationToken cancellationToken)
    {
        var users = _versityUsersRepository.GetAllUsers().ToList();
        var viewDtos = new List<ViewVersityUserDto>();
        foreach (var user in users)
            viewDtos.Add(ViewVersityUserDto.MapFromModel(user, await _versityUsersRepository.GetRolesAsync(user)));
        
        return viewDtos;
    }
}