using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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