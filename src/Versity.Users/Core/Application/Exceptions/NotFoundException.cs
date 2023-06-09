namespace Application.Exceptions;

public class NotFoundException<T> : Exception
{
    public NotFoundException(string key) : base($"Entity {typeof(T).FullName}, ({key} not found)")
    {
        
    }
}