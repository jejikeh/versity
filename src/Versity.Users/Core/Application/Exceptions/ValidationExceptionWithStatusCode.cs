using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class HttpValidationExceptionWithStatusCode : ExceptionWithStatusCode
{
    public HttpValidationExceptionWithStatusCode(object? value = null) : 
        base(
            StatusCodes.Status403Forbidden, 
            value)
    {
    }
}