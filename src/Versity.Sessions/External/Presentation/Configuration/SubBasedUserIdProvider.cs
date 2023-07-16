using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Presentation.Configuration;

public class SubBasedUserIdProvider : IUserIdProvider
{
    private readonly ILogger<SubBasedUserIdProvider> _logger;

    public SubBasedUserIdProvider(ILogger<SubBasedUserIdProvider> logger)
    {
        _logger = logger;
    }

    public string GetUserId(HubConnectionContext connectionContext)
    {
        _logger.LogInformation("--> Start looking for Name Identifier");
        var claims = connectionContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (claims is not null)
        {
            _logger.LogInformation("--> Claim was founded!");
            
            return claims.Value;
        }
        
        _logger.LogInformation("--> Claim was`t founded! Default to empty guid!");
        return new Guid().ToString();
    }
}