using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Auth.Queries;

public record GetAllRefreshTokensCommand : IRequest<IEnumerable<RefreshToken>>
{
    
}