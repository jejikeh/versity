using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.RequestHandlers.SessionLogging.Commands.CacheLogData;

public class CacheLogDataCommandHandler : IRequestHandler<CacheLogDataCommand>
{
    private readonly ICacheService _cacheService;

    public CacheLogDataCommandHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(CacheLogDataCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.SetAddAsync(CachingKeys.SessionLogs, request);
    }
}