namespace Application.Exceptions;

public class ExceptionWithStatusCode : Exception
{
    public int StatusCode { get; }
    public object? Value { get; }

    public ExceptionWithStatusCode(int statusCode, object? value = null)
    {
        StatusCode = statusCode;
        Value = value;
    }
}