using Application.Abstractions.Repositories;
using Bogus;
using Domain.Models;
using Utils = Application.Common.Utils;

namespace Users.Tests.Integrations.Helpers;

public static class VersityUserSeeder
{
    public static async Task<(VersityUser, string)> SeedUserDataAsync(IVersityUsersRepository repository, Func<VersityUser, VersityUser>? changeUserAction = null)
    {
        var faker = new Faker();
        var id = Guid.NewGuid().ToString(); 
        var password = faker.Internet.Password() + $"!{Utils.GenerateRandomString(2)}!p1A2";
        var email = faker.Internet.Email();

        var user = new VersityUser
        {
            Id = id,
            UserName = faker.Internet.UserName(),
            Email = email,
            NormalizedEmail = email.ToUpper(),
            EmailConfirmed = true,
            FirstName = faker.Name.FirstName(),
            LastName = faker.Name.LastName(),
            SecurityStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "+000000000000"
        };
        
        if (changeUserAction != null)
        {
            user = changeUserAction(user);
        }
        
        await repository.CreateUserAsync(user, password);
        
        return (user, password);
    }
}