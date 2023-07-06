using Application;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Presentation.Extensions;

public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDbContext(builder.Configuration)
            .AddRepositories()
            .AddApplication()
            .AddKafkaFlow()
            .AddKafkaServices()
            .AddJwtAuthentication(builder.Configuration)
            .AddEndpointsApiExplorer()
            .AddControllers();
        
        builder.Services.AddSwaggerGen(options =>
        {
            options.UseDateOnlyTimeOnlyStringConverters();
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            }); 
        });

        builder.Services.AddCors(options => options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        }));
        
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
        app.MapControllers();
        
        return app;
    }
}