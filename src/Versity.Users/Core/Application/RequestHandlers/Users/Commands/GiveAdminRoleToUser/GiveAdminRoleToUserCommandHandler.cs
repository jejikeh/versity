using System.Security.Claims;
using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;

public class GiveAdminRoleToUserCommandHandler : IRequestHandler<GiveAdminRoleToUserCommand, string>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IAuthTokenGeneratorService _tokenGeneratorService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    
    public GiveAdminRoleToUserCommandHandler(IVersityUsersRepository versityUsersRepository, IAuthTokenGeneratorService tokenGeneratorService, IHttpContextAccessor httpContextAccessor)
    {
        _versityUsersRepository = versityUsersRepository;
        _tokenGeneratorService = tokenGeneratorService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(GiveAdminRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            userId = _httpContextAccessor.HttpContext?.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(userId)) 
            { 
                throw new InvalidOperationException("User claims was empty!");
            }
        }

        var versityUser = await _versityUsersRepository.GetUserByIdAsync(userId);
        if (versityUser is null) 
        {
            throw new IncorrectEmailOrPasswordExceptionWithStatusCode();
        }
        await _versityUsersRepository.SetUserRoleAsync(versityUser, VersityRole.Admin);
        var userRoles = await _versityUsersRepository.GetUserRolesAsync(versityUser);
        
        return _tokenGeneratorService.GenerateToken(versityUser.Id, versityUser.NormalizedEmail, userRoles);
    }
}