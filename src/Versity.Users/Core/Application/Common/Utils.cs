using Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Application.Common;

public static class IdentityResultHelpers
{
    public static void AggregateIdentityErrorsAndThrow(IdentityResult result)
    {
        if (!result.Succeeded)
        {   
            var errors = result.Errors.Aggregate(string.Empty, (current, error) => current + (error.Description + "\n"));
            throw new IdentityExceptionWithStatusCode(errors);
        }
    }
}