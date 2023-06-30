using System.Text;
using Application.Abstractions.Repositories;
using Application.Common;
using Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, IdentityResult>
{
    private readonly IVersityUsersRepository _versityUsersRepository;

    public ConfirmEmailCommandHandler(IVersityUsersRepository versityUsersRepository)
    {
        _versityUsersRepository = versityUsersRepository;
    }

    public async Task<IdentityResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(request.UserId);
        if (versityUser is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }
        
        var result = await _versityUsersRepository.ConfirmUserEmail(versityUser, request.Token);
        Utils.AggregateIdentityErrorsAndThrow(result);

        return result;
    }
}