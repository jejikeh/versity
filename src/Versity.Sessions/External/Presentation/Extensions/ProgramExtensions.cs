﻿using Application;
using Application.Abstractions;
using Hangfire;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi.Models;
using Presentation.Configuration;
using Serilog;

namespace Presentation.Extensions;

public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDbContext(builder.Configuration)
            .AddRepositories()
            .AddRedisCaching()
            .AddApplication()
            .AddHttpContextAccessor()
            .AddJwtAuthentication(builder.Configuration)
            .AddSwagger()
            .AddCors(options => options.ConfigureAllowAllCors())
            .AddKafka(new KafkaConsumerConfiguration())
            .AddHangfireService()
            .AddEndpointsApiExplorer()
            .AddControllers();
        
        builder.Services.AddScoped<IVersityUsersDataService, GrpcUsersDataService>();
        
        return builder;
    }
    
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error-development");
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("AllowAll");
        app.UseHangfireDashboard();
        app.MapControllers();
        
        RecurringJob.AddOrUpdate<UpdateSessionStatusService>(
            "ExpireExpiredSessions",
            x => x.ExpireExpiredSessions(), 
            Cron.Minutely);
        
        RecurringJob.AddOrUpdate<UpdateSessionStatusService>(
            "OpenInactiveSessions",
            x => x.OpenInactiveSessions(), 
            Cron.Minutely);

        return app;
    }
    
    private static CorsOptions ConfigureAllowAllCors(this CorsOptions options)
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        });
        
        return options;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
        });

        return serviceCollection;
    }
}