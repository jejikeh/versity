using Versity.ApiGateway.Extensions;

namespace Versity.ApiGateway;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .ConfigureBuilder();

        var application = await builder
            .Build()
            .ConfigureApplication();

        await application.RunApplicationAsync();
    }
}