using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Users.Queries.GetVersityUserById;

public class GetVersityUserByIdCommandHandler 
    : IRequestHandler<GetVersityUserByIdCommand, ViewVersityUserDto>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public GetVersityUserByIdCommandHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<ViewVersityUserDto> Handle(GetVersityUserByIdCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(request.Id);
        if (versityUser == null)
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");

        return ViewVersityUserDto.MapFromModel(
            versityUser, 
            await _versityUsersRepository.GetRolesAsync(versityUser));
    }
}