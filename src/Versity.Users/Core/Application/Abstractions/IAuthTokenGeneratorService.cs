using Domain.Models;

namespace Application.Abstractions;

public interface IAuthTokenGeneratorService
{
    string GenerateToken(string userId, string userEmail, params string[] roles);
}