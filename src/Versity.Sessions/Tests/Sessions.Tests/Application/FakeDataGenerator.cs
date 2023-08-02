﻿using Application.Dtos;
using Application.RequestHandlers.SessionLogging.Commands.CacheLogData;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogData;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogsData;
using Application.RequestHandlers.Sessions.Commands.CreateSession;
using Bogus;
using Domain.Models;
using Domain.Models.SessionLogging;

namespace Sessions.Tests.Application;

public static class FakeDataGenerator
{
    public static Product GenerateFakeProduct()
    {
        return new Faker<Product>().CustomInstantiator(faker => new Product() {
            Id = Guid.NewGuid(),
            ExternalId = Guid.NewGuid(),
            Title = faker.Commerce.ProductName()
        }).Generate();
    }
    
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
    
    public static async IAsyncEnumerable<CacheLogDataCommand> GenerateAsyncFakeCacheLogDataCommands(int count)
    {
        var commands =  new Faker<CacheLogDataCommand>().CustomInstantiator(faker => 
                new CacheLogDataCommand(
                    Guid.NewGuid(), 
                    faker.Date.Past(),
                    (LogLevel)new Random().Next(3),
                    faker.Lorem.Sentence()))
            .Generate(count);

        foreach (var command in commands)
        {
            yield return command;
        }
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
    
    public static Session GenerateFakeSession(int logCount)
    {
        return new Faker<Session>().CustomInstantiator(faker => new Session()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            Status = (SessionStatus)new Random().Next(5),
            Product = GenerateFakeProduct(),
            Logs = GenerateFakeSessionLogs(logCount),
            Expiry = faker.Date.Past(),
            Start = faker.Date.Future()
        }).Generate();
    }
    
    public static GetSessionByIdViewModel GenerateFakeSessionByIdViewModel(int logCount)
    {
        return new Faker<GetSessionByIdViewModel>().CustomInstantiator(faker => new GetSessionByIdViewModel(
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            GenerateFakeProduct(),
            Guid.NewGuid(),
            faker.Date.Past(),
            faker.Date.Future(),
            (SessionStatus)new Random().Next(5)
        )).Generate();
    }
    
    public static List<Session> GenerateFakeSessions(int count, int logCount)
    {
        return new Faker<Session>().CustomInstantiator(faker => new Session()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            Status = (SessionStatus)new Random().Next(5),
            Product = GenerateFakeProduct(),
            Logs = GenerateFakeSessionLogs(logCount),
            Expiry = faker.Date.Future(),
            Start = faker.Date.Past()
        }).Generate(count);
    }

    public static CreateSessionCommand GenerateFakeCreateSessionCommand()
    {
        return new Faker<CreateSessionCommand>().CustomInstantiator(faker => new CreateSessionCommand(
            Guid.NewGuid().ToString(),
            Guid.NewGuid(),
            faker.Date.Between(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddYears(2)),
            faker.Date.Between(DateTime.UtcNow.AddYears(2), DateTime.UtcNow.AddYears(3))
        )).Generate();
    }

    public static UserSessionsViewModel GenerateFakeUserSessionsViewModel()
    {
        return new Faker<UserSessionsViewModel>().CustomInstantiator(faker => new UserSessionsViewModel(
            Guid.NewGuid(),
            faker.Commerce.ProductName(),
            Guid.NewGuid(),
            faker.Date.Future(),
            faker.Date.Past(),
            (SessionStatus)new Random().Next(5)))
            .Generate();
    }
    
    public static List<UserSessionsViewModel> GenerateFakeUsersSessionsViewModel(int count)
    {
        return new Faker<UserSessionsViewModel>().CustomInstantiator(faker => new UserSessionsViewModel(
                Guid.NewGuid(),
                faker.Commerce.ProductName(),
                Guid.NewGuid(),
                faker.Date.Future(),
                faker.Date.Past(),
                (SessionStatus)new Random().Next(5)))
            .Generate(count);
    }
    
    public static SessionViewModel GenerateFakeSessionViewModel()
    {
        return new Faker<SessionViewModel>().CustomInstantiator(faker => new SessionViewModel(
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid(), 
                Guid.NewGuid(),
                faker.Date.Future(),
                faker.Date.Past(),
                (SessionStatus)new Random().Next(5)))
            .Generate();
    }
    
    public static List<SessionViewModel> GenerateFakeSessionViewModels(int count)
    {
        return new Faker<SessionViewModel>().CustomInstantiator(faker => new SessionViewModel(
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid(), 
                Guid.NewGuid(),
                faker.Date.Future(),
                faker.Date.Past(),
                (SessionStatus)new Random().Next(5)))
            .Generate(count);
    }
}   