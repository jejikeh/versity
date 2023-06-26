using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IVersityRefreshTokensRepository
{
    public Task<IEnumerable<RefreshToken>> GetAllAsync(CancellationToken cancellationToken);
    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
    public RefreshToken Update(RefreshToken token);
    public Task<IEnumerable<RefreshToken>> GetAllUserTokensByUserIdAsync(string userId, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}