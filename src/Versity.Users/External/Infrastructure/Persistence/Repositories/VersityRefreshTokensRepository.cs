using Application.Abstractions.Repositories;
using Domain.Models;

namespace Infrastructure.Persistence.Repositories;

public class VersityRefreshTokensRepository : IVersityRefreshTokensRepository
{
    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}