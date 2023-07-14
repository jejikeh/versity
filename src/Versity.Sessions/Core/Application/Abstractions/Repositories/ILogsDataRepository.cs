using Domain.Models.SessionLogging;

namespace Application.Abstractions.Repositories;

public interface ILogsDataRepository
{
    public IQueryable<LogData> GetAllLogsData();
    public Task<LogData?> GetLogDataByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<LogData> CreateLogDataAsync(LogData logData, CancellationToken cancellationToken);
    public Task CreateRangeLogsDataAsync(IEnumerable<LogData> logsData, CancellationToken cancellationToken);
    public Task<List<LogData>> ToListAsync(IQueryable<LogData> logsData);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}