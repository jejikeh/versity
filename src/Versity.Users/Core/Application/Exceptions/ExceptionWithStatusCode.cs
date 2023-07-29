namespace Application.Exceptions;

public class ExceptionWithStatusCode : Exception
{
    public int StatusCode { get; }
    public string? ErrorMessage { get; }

    public ExceptionWithStatusCode(int statusCode, string? errorMessage = null) : base(errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}