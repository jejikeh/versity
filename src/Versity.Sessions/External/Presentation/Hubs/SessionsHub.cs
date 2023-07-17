using Application.Abstractions;
using Application.Abstractions.Hubs;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class SessionsHub : Hub<ISessionsHubClient>
{
    private readonly ISender _sender;
    private readonly ICacheService _cacheService;

    public SessionsHub(ISender sender, ICacheService cacheService)
    {
        _sender = sender;
        _cacheService = cacheService;
    }
    
    [Authorize(Roles = "Member")]
    public async Task UploadStream(IAsyncEnumerable<CreateLogDataCommand> stream)
    {
        await foreach (var item in stream)
        {
            await _cacheService.SetAddAsync("session-logs", item);
        }
    }
}