using System.Text;
using Application;
using Application.Abstractions;
using Domain.Models;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddIdentity<VersityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
    })
    .AddEntityFrameworkStores<VersityUsersDbContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>();

builder.Services.AddTransient<IAuthTokenGeneratorService, JwtTokenGeneratorService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        // TODO: move this from config to env variables
        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
    };
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
});

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseExceptionHandler("/error-development");
}
app.UseExceptionHandler("/error");
    

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
try
{
    var versityUsersDbContext = serviceProvider.GetRequiredService<VersityUsersDbContext>();
    versityUsersDbContext.Database.EnsureCreated();
    
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = Enum.GetNames(typeof(VersityRoles));
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}
catch (Exception _)
{
    // TODO
}

app.Run();