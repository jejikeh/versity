using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions.AuthExceptions;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.GiveAdminRoleToUser;

public class GiveAdminRoleToUserCommandHandler : IRequestHandler<GiveAdminRoleToUserCommand, string>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IAuthTokenGeneratorService _tokenGeneratorService;
    
    public GiveAdminRoleToUserCommandHandler(IVersityUsersRepository versityUsersRepository, IAuthTokenGeneratorService tokenGeneratorService)
    {
        _versityUsersRepository = versityUsersRepository;
        _tokenGeneratorService = tokenGeneratorService;
    }

    public async Task<string> Handle(GiveAdminRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(request.UserId);
        if (versityUser is null)
            throw new IncorrectEmailOrPasswordExceptionWithStatusCode();

        await _versityUsersRepository.SetUserRoleAsync(versityUser, VersityRole.Admin);
        var userRoles = await _versityUsersRepository.GetUserRolesAsync(versityUser);
        return _tokenGeneratorService.GenerateToken(versityUser.Id, versityUser.NormalizedEmail, userRoles);
    }
}