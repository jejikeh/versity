using Infrastructure.Persistence;
using Presentation.Extensions;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .ConfigureBuilder();
        
        var app = builder
            .Build()
            .ConfigureApplication();

        await app.RunApplicationAsync();
    }
}