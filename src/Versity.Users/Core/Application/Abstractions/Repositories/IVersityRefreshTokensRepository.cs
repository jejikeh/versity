using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IVersityRefreshTokenRepository
{
    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}