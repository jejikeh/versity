using System.Text.Json;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;

public class CreateProductMessageHandler : IKafkaMessageHandler
{
    private readonly IProductsRepository _productsRepository;
    private readonly IKafkaConsumerConfiguration _configuration;

    public CreateProductMessageHandler(IProductsRepository productsRepository, IKafkaConsumerConfiguration configuration)
    {
        _productsRepository = productsRepository;
        _configuration = configuration;
    }

    public async Task Handle(string key, string message, CancellationToken cancellationToken)
    {
        if (key != _configuration.CreateProductTopic)
        {
            return;
        }
        
        var request = JsonSerializer.Deserialize<CreateProductMessage>(message);
        
        if (await _productsRepository.GetProductByExternalIdAsync(request.Id, cancellationToken) is not null)
        {
            throw new ExceptionWithStatusCode(StatusCodes.Status409Conflict, "Product with specified Id external already exist!");
        }
        
        var product = new Product()
        {
            Id = Guid.NewGuid(),
            ExternalId = request.Id,
            Title = request.Title
        };
        
        await _productsRepository.CreateProductAsync(product, cancellationToken);
        await _productsRepository.SaveChangesAsync(cancellationToken);
    }
}