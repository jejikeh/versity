using Versity.Users.Core.Domain.Models;

namespace Versity.Users.Core.Application.Abstractions;

public interface IAuthTokenGeneratorService
{
    string GenerateToken(VersityUser user);
}