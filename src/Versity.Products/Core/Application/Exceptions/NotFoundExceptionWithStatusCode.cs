using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class NotFoundExceptionWithStatusCode : ExceptionWithStatusCode
{
    public NotFoundExceptionWithStatusCode(string message) : base(StatusCodes.Status404NotFound, message)
    {
    }
}