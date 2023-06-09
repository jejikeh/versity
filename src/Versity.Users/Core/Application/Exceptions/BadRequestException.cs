using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class BadRequestException : HttpResponseException
{
    public BadRequestException(object? value = null) : 
        base(StatusCodes.Status400BadRequest, value)
    {
    }
}