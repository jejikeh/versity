using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.RequestHandlers.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IVersityProductsRepository _products;
    private readonly IProductProducerService _productProducerService;

    public CreateProductCommandHandler(IVersityProductsRepository products, IProductProducerService productProducerService)
    {
        _products = products;
        _productProducerService = productProducerService;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productId = Guid.NewGuid();
        while (await _products.GetProductByIdAsync(productId, cancellationToken) is not null)
        {
            productId = Guid.NewGuid();
        }
        
        var product = new Product()
        {
            Id = productId,
            Title = request.Title,
            Description = request.Description,
            Author = request.Author,
            Release = request.Release
        };
          
        var result = await _products.CreateProductAsync(product, cancellationToken);
        await _products.SaveChangesAsync(cancellationToken);
        await _productProducerService.CreateProductProduce(CreateProductProduceDto.CreateFromModel(product), cancellationToken);
        
        return result;
    }
}