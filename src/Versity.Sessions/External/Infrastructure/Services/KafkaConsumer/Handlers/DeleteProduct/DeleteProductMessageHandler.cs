using System.Text.Json;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Infrastructure.Services.KafkaConsumer.Abstractions;

namespace Infrastructure.Services.KafkaConsumer.Handlers.DeleteProduct;

public class DeleteProductMessageHandler : IKafkaMessageHandler
{
    private readonly IProductsRepository _productsRepository;

    public DeleteProductMessageHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public async Task Handle(string key, string message, CancellationToken cancellationToken)
    {
        if (key != "DeleteProduct")
        {
            return;
        }
        
        var request = JsonSerializer.Deserialize<DeleteProductMessage>(message);
        
        var product = await _productsRepository.GetProductByExternalIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no product with this External Id");
        }
        
        _productsRepository.DeleteProduct(product);
        await _productsRepository.SaveChangesAsync(cancellationToken);
    }
}