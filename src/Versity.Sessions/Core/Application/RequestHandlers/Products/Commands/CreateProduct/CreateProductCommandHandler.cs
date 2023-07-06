using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestHandlers.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductsRepository _productsRepository;

    public CreateProductCommandHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
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
        
        var result = await _productsRepository.CreateProductAsync(product, cancellationToken);
        await _productsRepository.SaveChangesAsync(cancellationToken);

        return result;
    }
}