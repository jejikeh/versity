using Infrastructure.Persistence;
using Presentation.Extensions;

namespace Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .ConfigureBuilder()
            .AddLogging();

        var application = builder
            .Build()
            .ConfigureApplication();

        await application.RunApplicationAsync();
    }
}