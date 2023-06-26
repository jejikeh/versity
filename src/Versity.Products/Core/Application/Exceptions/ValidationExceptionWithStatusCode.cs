using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class ValidationExceptionWithStatusCode : ExceptionWithStatusCode
{
    public ValidationExceptionWithStatusCode(object? value = null) : 
        base(
            StatusCodes.Status403Forbidden, 
            value)
    {
    }
}