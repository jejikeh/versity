namespace Application.Exceptions;

public class CustomException : Exception
{
    public int StatusCode { get; }
    public object? Value { get; }

    public CustomException(int statusCode, object? value = null)
    {
        StatusCode = statusCode;
        Value = value;
    }
}