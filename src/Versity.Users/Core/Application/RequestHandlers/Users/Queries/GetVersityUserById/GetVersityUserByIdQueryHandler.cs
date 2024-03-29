﻿using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using MediatR;

namespace Application.RequestHandlers.Users.Queries.GetVersityUserById;

public class GetVersityUserByIdQueryHandler : IRequestHandler<GetVersityUserByIdQuery, ViewVersityUserDto>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public GetVersityUserByIdQueryHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<ViewVersityUserDto> Handle(GetVersityUserByIdQuery request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(request.Id);
        if (versityUser is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }
        
        return ViewVersityUserDto.MapFromModel(
            versityUser, 
            await _versityUsersRepository.GetUserRolesAsync(versityUser));
    }
}