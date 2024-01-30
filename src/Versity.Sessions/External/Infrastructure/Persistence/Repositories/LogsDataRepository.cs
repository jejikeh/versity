using Application.Abstractions.Repositories;
using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class LogsDataRepository : ILogsDataRepository
{
    private readonly VersitySessionsServiceDbContext _context;

    public LogsDataRepository(VersitySessionsServiceDbContext context)
    {
        _context = context;
    }

    public IQueryable<LogData> GetAllLogsData()
    {
        return _context.LogsData.AsQueryable();
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

    public async Task<List<LogData>> ToListAsync(IQueryable<LogData> logsData)
    {
        return await logsData.ToListAsync();
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