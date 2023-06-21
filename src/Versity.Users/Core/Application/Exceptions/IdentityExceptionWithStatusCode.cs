using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class IdentityExceptionWithStatusCode : ExceptionWithStatusCode
{
    public IdentityExceptionWithStatusCode(string value) : base(StatusCodes.Status401Unauthorized, value)
    {
    }
}