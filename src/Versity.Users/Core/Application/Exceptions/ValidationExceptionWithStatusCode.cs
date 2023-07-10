using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class ValidationExceptionWithStatusCode : ExceptionWithStatusCode
{
    public ValidationExceptionWithStatusCode(string? errorMessage = null) : base(StatusCodes.Status403Forbidden, errorMessage)
    {
    }
}