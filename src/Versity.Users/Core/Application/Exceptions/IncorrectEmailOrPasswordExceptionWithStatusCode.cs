using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class IncorrectEmailOrPasswordExceptionWithStatusCode : ExceptionWithStatusCode
{
    public IncorrectEmailOrPasswordExceptionWithStatusCode() : 
        base(StatusCodes.Status401Unauthorized, "Email or password is incorrect! Please check your credentials and try again.")
    {
    }
}