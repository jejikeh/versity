namespace Application.Abstractions;

public interface IProductPublisher 
{
    public Task PublishProductAsync(string trigger, Guid id, string title, CancellationToken cancellationToken);
}