using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class BadRequestException : CustomException
{
    public BadRequestException(object? value = null) : 
        base(StatusCodes.Status400BadRequest, value)
    {
    }
}