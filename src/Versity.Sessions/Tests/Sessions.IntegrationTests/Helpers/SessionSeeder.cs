using Application.Abstractions.Repositories;
using Domain.Models;
using Domain.Models.SessionLogging;

namespace Sessions.IntegrationTests.Helpers;

public static class SessionSeeder
{
    public static async Task<(Domain.Models.Session, Product, SessionLogs, List<LogData>)> SeedSessionDataAsync(
        ISessionsRepository sessionsRepository,
        ISessionLogsRepository sessionLogsRepository,
        ILogsDataRepository logsDataRepository,
        IProductsRepository productsRepository,
        int count = 0)
    {
        var (session, product, sessionLogs, logDatas) =
            FakeDataGenerator.GenerateFakeSessionAndReturnAllDependEntities(count == 0 ? Random.Shared.Next(1, 20) : count);

        await productsRepository.CreateProductAsync(product, CancellationToken.None);
        await sessionsRepository.CreateSessionAsync(session, CancellationToken.None);
        await sessionLogsRepository.CreateSessionLogsAsync(sessionLogs, CancellationToken.None);
        await logsDataRepository.CreateRangeLogsDataAsync(logDatas, CancellationToken.None);

        await productsRepository.SaveChangesAsync(CancellationToken.None);
        await sessionsRepository.SaveChangesAsync(CancellationToken.None);
        await sessionLogsRepository.SaveChangesAsync(CancellationToken.None);
        await logsDataRepository.SaveChangesAsync(CancellationToken.None);

        return (session, product, sessionLogs, logDatas);
    }
    
    public static async Task<(Session, Product, SessionLogs, List<LogData>)> SeedSessionDataAsync(
        ISessionsRepository sessionsRepository,
        ISessionLogsRepository sessionLogsRepository,
        ILogsDataRepository logsDataRepository,
        IProductsRepository productsRepository,
        Guid userId,
        int count = 0)
    {
        var (session, product, sessionLogs, logDatas) =
            FakeDataGenerator.GenerateFakeSessionAndReturnAllDependEntitiesWithUser(userId, count == 0 ? Random.Shared.Next(1, 20) : count);

        await productsRepository.CreateProductAsync(product, CancellationToken.None);
        await sessionsRepository.CreateSessionAsync(session, CancellationToken.None);
        await sessionLogsRepository.CreateSessionLogsAsync(sessionLogs, CancellationToken.None);
        await logsDataRepository.CreateRangeLogsDataAsync(logDatas, CancellationToken.None);

        await productsRepository.SaveChangesAsync(CancellationToken.None);
        await sessionsRepository.SaveChangesAsync(CancellationToken.None);
        await sessionLogsRepository.SaveChangesAsync(CancellationToken.None);
        await logsDataRepository.SaveChangesAsync(CancellationToken.None);

        return (session, product, sessionLogs, logDatas);
    }
    
    public static async Task<List<(Session, Product, SessionLogs, List<LogData>)>> SeedSessionsDataAsync(
        ISessionsRepository sessionsRepository,
        ISessionLogsRepository sessionLogsRepository,
        ILogsDataRepository logsDataRepository,
        IProductsRepository productsRepository,
        int count = 0)
    {
        var fakeData =
            FakeDataGenerator.GenerateFakeSessionsAndReturnAllDependEntities(Random.Shared.Next(1, 10), Random.Shared.Next(1, 10));

        await productsRepository.CreateRangeProductAsync(fakeData.Select(x => x.Item2).ToArray(), CancellationToken.None);
        await sessionsRepository.CreateSessionRangeAsync(fakeData.Select(x => x.Item1).ToArray(), CancellationToken.None);
        await sessionLogsRepository.CreateSessionLogsRangeAsync(fakeData.Select(x => x.Item3).ToArray(), CancellationToken.None);
        await logsDataRepository.CreateRangeLogsDataAsync(fakeData.SelectMany(x => x.Item4), CancellationToken.None);

        await productsRepository.SaveChangesAsync(CancellationToken.None);
        await sessionsRepository.SaveChangesAsync(CancellationToken.None);
        await sessionLogsRepository.SaveChangesAsync(CancellationToken.None);
        await logsDataRepository.SaveChangesAsync(CancellationToken.None);

        return fakeData;
    }
}