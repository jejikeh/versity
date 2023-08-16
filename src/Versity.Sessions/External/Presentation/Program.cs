using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Presentation.Configuration;
using Presentation.Extensions;

namespace Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var applicationConfiguration = new ApplicationConfiguration(builder.Configuration);
        
        builder
            .ConfigureBuilder(applicationConfiguration)
            .AddLogging(applicationConfiguration);

        var application = builder
            .Build()
            .ConfigureApplication();

        await application.RunApplicationAsync(applicationConfiguration);
    }
}