using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.AuthExceptions;

public class IncorrectEmailOrPasswordException : HttpResponseException
{
    public IncorrectEmailOrPasswordException() : 
        base(
            StatusCodes.Status401Unauthorized, 
            "Email or password is incorrect! Please check your credentials and try again.")
    {
    }
}