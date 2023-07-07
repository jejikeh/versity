using System.Text.Json;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using Infrastructure.KafkaConsumerService.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.KafkaConsumerService.Handlers.CreateProduct;

public class CreateProductMessageHandler : IKafkaMessageHandler
{
    private readonly IProductsRepository _productsRepository;

    public CreateProductMessageHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public async Task Handle(string key, string message, CancellationToken cancellationToken)
    {
        if (key != "CreateProduct")
        {
            return;
        }
        
        var request = JsonSerializer.Deserialize<CreateProductMessage>(message);
        
        if (await _productsRepository.GetProductByExternalIdAsync(request.Id, cancellationToken) is not null)
        {
            throw new ExceptionWithStatusCode(StatusCodes.Status409Conflict, "Product with specified Id external already exist!");
        }
        
        var productId = Guid.NewGuid();
        while (await _productsRepository.GetProductByIdAsync(productId, cancellationToken) is not null)
        {
            productId = Guid.NewGuid();
        }
        
        var product = new Product()
        {
            Id = productId,
            ExternalId = request.Id,
            Title = request.Title
        };
        
        await _productsRepository.CreateProductAsync(product, cancellationToken);
        await _productsRepository.SaveChangesAsync(cancellationToken);
    }
}