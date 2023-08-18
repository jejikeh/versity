using System.Reflection;
using Domain.Models;
using Domain.Models.SessionLogging;
using Infrastructure.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public class VersitySessionsServiceMongoDbContext
{
    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }

    public IMongoCollection<Product> Products { get; }
    public IMongoCollection<Session> Sessions { get; }
    public IMongoCollection<SessionLogs> SessionLogs { get; }
    public IMongoCollection<LogData> LogsData { get; }


    public VersitySessionsServiceMongoDbContext(IApplicationConfiguration applicationConfiguration)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("MONGO_HOST: " + applicationConfiguration.DatabaseConnectionString);
        
        Client = new MongoClient(applicationConfiguration.DatabaseConnectionString);
        Database = Client.GetDatabase(applicationConfiguration.DatabaseName);

        Products = Database.GetCollection<Product>("products");
        Sessions = Database.GetCollection<Session>("sessions");
        SessionLogs = Database.GetCollection<SessionLogs>("session-logs");
        LogsData = Database.GetCollection<LogData>("log-datas");
    }
}