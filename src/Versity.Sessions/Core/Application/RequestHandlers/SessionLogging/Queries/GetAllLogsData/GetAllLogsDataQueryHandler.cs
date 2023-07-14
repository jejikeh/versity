using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models.SessionLogging;
using MediatR;

namespace Application.RequestHandlers.SessionLogging.Queries.GetAllLogsData;

public class GetAllLogsDataQueryHandler : IRequestHandler<GetAllLogsDataQuery, IEnumerable<LogData>>
{
    private readonly ILogsDataRepository _logsDataRepository;

    public GetAllLogsDataQueryHandler(ILogsDataRepository logsDataRepository)
    {
        _logsDataRepository = logsDataRepository;
    }

    public async Task<IEnumerable<LogData>> Handle(GetAllLogsDataQuery request, CancellationToken cancellationToken)
    {
        var logsData = _logsDataRepository
            .GetAllLogsData()
            .OrderBy(x => x.Id)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);

        return await _logsDataRepository.ToListAsync(logsData);
    }
}