using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class BadRequestExceptionWithStatusCode : ExceptionWithStatusCode
{
    public BadRequestExceptionWithStatusCode(object? value = null) : 
        base(StatusCodes.Status400BadRequest, value)
    {
    }
}