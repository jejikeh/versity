using Application.Abstractions.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class SessionsHub : Hub<ISessionsHubClient>
{
    private readonly ILogger<SessionsHub> _logger;

    public SessionsHub(ILogger<SessionsHub> logger)
    {
        _logger = logger;
    }
    
    [Authorize(Roles = "Member")]
    public async Task UploadStream(IAsyncEnumerable<string> stream)
    {
        await foreach (var item in stream)
        {
            _logger.LogInformation("--> New message in stream " + item);
        }
    }
}