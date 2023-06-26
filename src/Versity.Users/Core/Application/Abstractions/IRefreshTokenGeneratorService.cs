using Domain.Models;

namespace Application.Abstractions;

public interface IRefreshTokenGeneratorService
{
    public RefreshToken GenerateToken(string userId);
    public Task<RefreshToken> ValidateTokenAsync(string userId, string token, CancellationToken cancellationToken);
}