using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IVersityRefreshTokensRepository
{
    public Task<IEnumerable<RefreshToken>> GetAllAsync(CancellationToken cancellationToken);
    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
    public RefreshToken Update(RefreshToken token);
    public Task<RefreshToken> FindUserTokenAsync(string userId, string token, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}