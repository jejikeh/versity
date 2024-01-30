using Application.Abstractions.Hubs;
using Application.RequestHandlers.SessionLogging.Commands.CacheLogData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class SessionsHub : Hub<ISessionsHubClient>
{
    private readonly ISender _sender;

    public SessionsHub(ISender sender)
    {
        _sender = sender;
    }
    
    [Authorize(Roles = "Member")]
    public async Task UploadStream(IAsyncEnumerable<CacheLogDataCommand> stream)
    {
        await foreach (var item in stream)
        {
            await _sender.Send(item);
        }
    }
}