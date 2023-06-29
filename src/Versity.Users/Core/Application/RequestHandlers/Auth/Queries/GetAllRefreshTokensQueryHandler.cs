using Application.Abstractions.Repositories;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Auth.Queries;

public class GetAllRefreshTokensCommandHandler : IRequestHandler<GetAllRefreshTokensQuery, IEnumerable<RefreshToken>>
{
    private readonly IVersityRefreshTokensRepository _tokensRepository;

    public GetAllRefreshTokensCommandHandler(IVersityRefreshTokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task<IEnumerable<RefreshToken>> Handle(GetAllRefreshTokensQuery request, CancellationToken cancellationToken)
    {
        return await _tokensRepository.GetAllAsync(cancellationToken);;
    }
}