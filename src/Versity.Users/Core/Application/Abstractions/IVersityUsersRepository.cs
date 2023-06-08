using Versity.Users.Core.Domain.Models;

namespace Versity.Users.Core.Application.Abstractions;

public interface IVersityUsersRepository
{
    public Task<IEnumerable<VersityUser?>> GetAllUsersAsync(CancellationToken cancellationToken);
    public Task<VersityUser?> GetUserAsync(Guid id, CancellationToken cancellationToken);
    public Task CreateUserAsync(VersityUser user, CancellationToken cancellationToken);
    public void UpdateUserAsync(VersityUser user);
    public void DeleteUserAsync(VersityUser user);
    public Task SaveAsync(CancellationToken cancellationToken);
}