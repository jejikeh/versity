namespace Application.Exceptions.AuthExceptions;

public class InvalidEmailOrPasswordException : HttpResponseException
{
    public InvalidEmailOrPasswordException() : base(statusCode, "")
    {
    }
}