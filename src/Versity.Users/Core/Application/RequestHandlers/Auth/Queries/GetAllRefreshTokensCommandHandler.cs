using Application.Abstractions.Repositories;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Auth.Queries;

public class GetAllRefreshTokensCommandHandler : IRequestHandler<GetAllRefreshTokensCommand, IEnumerable<RefreshToken>>
{
    private readonly IVersityRefreshTokensRepository _tokensRepository;

    public GetAllRefreshTokensCommandHandler(IVersityRefreshTokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task<IEnumerable<RefreshToken>> Handle(GetAllRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var tokens = await _tokensRepository.GetAllAsync(cancellationToken);
        return tokens;
    }
}