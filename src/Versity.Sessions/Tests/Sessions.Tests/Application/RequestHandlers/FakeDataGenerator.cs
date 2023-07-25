using Application.Dtos;
using Application.RequestHandlers.SessionLogging.Commands.CacheLogData;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogData;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogsData;
using Bogus;
using Domain.Models;
using Domain.Models.SessionLogging;

namespace Sessions.Tests.Application.RequestHandlers;

public static class FakeDataGenerator
{
    public static List<Product> GenerateFakeProducts(int cout)
    {
        return new Faker<Product>().CustomInstantiator(faker => new Product() {
            Id = Guid.NewGuid(),
            ExternalId = Guid.NewGuid(),
            Title = faker.Commerce.ProductName()
        }).Generate(cout);
    }
    
    public static CacheLogDataCommand GenerateFakeCacheLogDataCommand()
    {
        return new Faker<CacheLogDataCommand>().CustomInstantiator(faker => 
                new CacheLogDataCommand(
                    Guid.NewGuid(), 
                    faker.Date.Past(),
                    (LogLevel)new Random().Next(3),
                    faker.Lorem.Sentence()))
            .Generate();
    }
    
    public static CreateLogDataCommand GenerateFakeCreateLogDataCommand()
    {
        return new Faker<CreateLogDataCommand>().CustomInstantiator(faker => 
                new CreateLogDataCommand(
                    Guid.NewGuid(), 
                    faker.Date.Past(),
                    (LogLevel)new Random().Next(3),
                    faker.Lorem.Sentence()))
            .Generate();
    }

    public static SessionLogs GenerateFakeSessionLogs(int count)
    {
        return new Faker<SessionLogs>().CustomInstantiator(faker => new SessionLogs
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            Session = default!,
            Logs = GenerateFakeLogsData(count),
        }).Generate();
    }
    
    public static List<SessionLogs> GenerateFakeSessionsLogs(int count, int logCount)
    {
        return new Faker<SessionLogs>().CustomInstantiator(faker => new SessionLogs
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            Session = default!,
            Logs = GenerateFakeLogsData(logCount),
        }).Generate(count);
    }

    public static List<LogData> GenerateFakeLogsData(int count)
    {
        return new Faker<LogData>().CustomInstantiator(faker => new LogData
        {
            Id = Guid.NewGuid(),
            Time = faker.Date.Past(),
            LogLevel = (LogLevel)new Random().Next(3),
            Data = faker.Lorem.Sentence(),
            SessionLogsId = Guid.NewGuid(),
            SessionLogs = null!,

        }).Generate(count);
    }
    
    public static LogData GenerateFakeLogData()
    {
        return new Faker<LogData>().CustomInstantiator(faker => new LogData
        {
            Id = Guid.NewGuid(),
            Time = faker.Date.Past(),
            LogLevel = (LogLevel)new Random().Next(3),
            Data = faker.Lorem.Sentence(),
            SessionLogsId = Guid.NewGuid(),
            SessionLogs = null!,
        }).Generate();
    }
    
    public static CreateLogsDataCommand GenerateFakeCreateLogsDataCommand(int count)
    {
        return new Faker<CreateLogsDataCommand>().CustomInstantiator(faker =>
                new CreateLogsDataCommand(
                    Guid.NewGuid(),
                    GenerateFakeCreateLogDataDto(count)))
            .Generate();
    }
    
    public static List<CreateLogDataDto> GenerateFakeCreateLogDataDto(int count)
    {
        return new Faker<CreateLogDataDto>().CustomInstantiator(faker => 
                new CreateLogDataDto(
                    faker.Date.Past(),
                    (LogLevel)new Random().Next(3),
                    faker.Lorem.Sentence()))
            .Generate(count);
    }
}