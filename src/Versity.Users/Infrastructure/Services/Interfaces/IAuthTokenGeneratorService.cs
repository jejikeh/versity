using Versity.Users.Core.Domain.Models;

namespace Versity.Users.Infrastructure.Services.Interfaces;

public interface IAuthTokenGeneratorService
{
    string GenerateToken(VersityUser user, params string[] roles);
}