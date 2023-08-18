using Application.Abstractions.Repositories;
using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Persistence.MongoRepositories;

public class LogsDataMongoRepository : ILogsDataRepository
{
    private readonly VersitySessionsServiceMongoDbContext _context;

    public LogsDataMongoRepository(VersitySessionsServiceMongoDbContext context)
    {
        _context = context;
    }

    public IEnumerable<LogData> GetLogsData(int? skipCount, int? takeCount)
    {
        if (skipCount is null && takeCount is null)
        {
            return _context.LogsData
                .AsQueryable()
                .OrderBy(data => data.Time)
                .ToList();
        }
        
        return _context.LogsData
            .AsQueryable()
            .OrderBy(data => data.Time)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10)
            .ToList();
    }

    public async Task<LogData?> GetLogDataByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _context.LogsData.FindAsync(logData => logData.Id == id, cancellationToken: cancellationToken);
        
        return await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<LogData> CreateLogDataAsync(LogData logData, CancellationToken cancellationToken)
    {
        await _context.LogsData.InsertOneAsync(logData, cancellationToken: cancellationToken);

        return logData;
    }

    public async Task CreateRangeLogsDataAsync(IEnumerable<LogData> logsData, CancellationToken cancellationToken)
    {
        await _context.LogsData.InsertManyAsync(logsData, cancellationToken: cancellationToken);
    }

    public async Task<List<LogData>> ToListAsync(IQueryable<LogData> logsData)
    {
        return await logsData.ToListAsync();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    public void SaveChanges()
    {
    }
}