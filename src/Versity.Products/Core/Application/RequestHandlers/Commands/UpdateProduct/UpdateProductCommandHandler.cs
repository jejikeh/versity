using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.RequestHandlers.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product>
{
    private readonly IVersityProductsRepository _products;

    public UpdateProductCommandHandler(IVersityProductsRepository products)
    {
        _products = products;
    }

    public async Task<Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _products.GetProductByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }

        product.Author = request.Author ?? product.Author;
        product.Title = request.Title ?? product.Title;
        product.Description = request.Description ?? product.Description;
        product.Release = request.Release ?? product.Release;
        
        var updatedProduct = _products.UpdateProduct(product);
        await _products.SaveChangesAsync(cancellationToken);
        
        return updatedProduct;
    }
}