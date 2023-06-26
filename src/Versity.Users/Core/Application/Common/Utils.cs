using System.Text;
using Application.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Common;

public static class Utils
{
    public static void AggregateIdentityErrorsAndThrow(IdentityResult result)
    {
        if (!result.Succeeded)
        {   
            var errors = result.Errors.Aggregate(string.Empty, (current, error) => current + (error.Description + "\n"));
            throw new IdentityExceptionWithStatusCode(errors);
        }
    }

    public const string AlphaNumeric  = "0123456789_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    
    public static string GenerateRandomString(int length)
    {
        var random = new Random();
        var stringBuilder = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            stringBuilder.Append(AlphaNumeric[random.Next(AlphaNumeric.Length)]);
        }

        return stringBuilder.ToString();
    }
}