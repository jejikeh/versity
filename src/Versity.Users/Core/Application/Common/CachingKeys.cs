using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Application.Common;

public static class CachingKeys
{
    public const string Tokens = "tokens";
    public static string UserToken(string token, string userId) => $"token-{token}-user-{userId}";
}