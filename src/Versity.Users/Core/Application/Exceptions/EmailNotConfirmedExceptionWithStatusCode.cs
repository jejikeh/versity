using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class EmailNotConfirmedExceptionWithStatusCode : ExceptionWithStatusCode
{
    public EmailNotConfirmedExceptionWithStatusCode() : 
        base(StatusCodes.Status401Unauthorized, "Email is not confirmed! Please confirm your Email.")
    {
    }
}