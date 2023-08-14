using Application.Abstractions.Repositories;
using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Persistence.SqlRepositories;

public class LogsDataSqlRepository : ILogsDataRepository
{
    private readonly VersitySessionsServiceSqlDbContext _context;

    public LogsDataSqlRepository(VersitySessionsServiceSqlDbContext context)
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
        return await _context.LogsData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<LogData> CreateLogDataAsync(LogData logData, CancellationToken cancellationToken)
    {
        var entity = await _context.AddAsync(logData, cancellationToken);

        return entity.Entity;
    }

    public async Task CreateRangeLogsDataAsync(IEnumerable<LogData> logsData, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(logsData, cancellationToken);
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}