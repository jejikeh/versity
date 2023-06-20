using Application.Abstractions.Repositories;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.RequestHandlers.Queries.GetProductById;

public class GetProductByIdRequestHandler : IRequestHandler<GetProductByIdQuery, Result<Product>>
{
    private readonly IVersityProductsRepository _products;

    public GetProductByIdRequestHandler(IVersityProductsRepository products)
    {
        _products = products;
    }

    public async Task<Result<Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _products.GetProductByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Fail("There is no product with this Id");
        }

        return product;
    }
}