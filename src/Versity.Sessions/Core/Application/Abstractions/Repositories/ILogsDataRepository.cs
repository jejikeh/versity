using Domain.Models.SessionLogging;

namespace Application.Abstractions.Repositories;

public interface ILogsDataRepository
{
    public IEnumerable<LogData> GetLogsData(int? skipCount, int? takeCount);
    public Task<LogData?> GetLogDataByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<LogData> CreateLogDataAsync(LogData logData, CancellationToken cancellationToken);
    public Task CreateRangeLogsDataAsync(IEnumerable<LogData> logsData, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
    public void SaveChanges();
}