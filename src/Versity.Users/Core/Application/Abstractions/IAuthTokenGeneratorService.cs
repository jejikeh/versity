using Domain.Models;

namespace Application.Abstractions;

public interface IAuthTokenGeneratorService
{
    public string GenerateToken(string userId, string userEmail, params string[] roles);
}