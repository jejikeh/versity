using Application.Abstractions.Repositories;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.RequestHandlers.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<Product>>
{
    private readonly IVersityProductsRepository _products;

    public UpdateProductCommandHandler(IVersityProductsRepository products)
    {
        _products = products;
    }

    public async Task<Result<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _products.GetProductByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Fail("There is no product with this Id");
        }

        product.Author = request.Author ?? product.Author;
        product.Title = request.Title ?? product.Title;
        product.Description = request.Description ?? product.Description;
        product.Release = request.Release ?? product.Release;
        product.Author = request.Author ?? product.Author;

        var updatedProduct = _products.UpdateProduct(product);
        return Result.Ok(updatedProduct);
    }
}