using Application.Abstractions.Repositories;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.RequestHandlers.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IVersityProductsRepository _products;

    public CreateProductCommandHandler(IVersityProductsRepository products)
    {
        _products = products;
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
        
        return result;
    }
}