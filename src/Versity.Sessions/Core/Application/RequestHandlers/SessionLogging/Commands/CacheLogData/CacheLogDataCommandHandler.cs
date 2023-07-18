using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.RequestHandlers.SessionLogging.Commands.CacheLogData;

public class CacheLogDataCommandHandler : IRequestHandler<CacheLogDataCommand>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheLogDataCommandHandler> _logger;

    public CacheLogDataCommandHandler(ICacheService cacheService, ILogger<CacheLogDataCommandHandler> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task Handle(CacheLogDataCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.SetAddAsync("session-logs", request);
    }
}