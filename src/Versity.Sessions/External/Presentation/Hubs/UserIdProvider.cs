using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Presentation.Hubs;

public class UserIdProvider
{
    public string GetUserId(HubConnectionContext connectionContext)
    {
        return connectionContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
    }
}