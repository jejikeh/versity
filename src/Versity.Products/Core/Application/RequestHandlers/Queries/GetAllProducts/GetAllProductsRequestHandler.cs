using Application.Abstractions.Repositories;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.RequestHandlers.Queries.GetAllProducts;

public class GetAllProductsRequestHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<Product>>>
{
    private readonly IVersityProductsRepository _products;

    public GetAllProductsRequestHandler(IVersityProductsRepository products)
    {
        _products = products;
    }

    public Task<Result<IEnumerable<Product>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = _products
            .GetAllProducts()
            .OrderBy(x => x.Title)
            .Skip(10 * (request.Page - 1))
            .Take(10)
            .ToList();

        return Task.Run(() => Result.Ok((IEnumerable<Product>)products), cancellationToken);
    }
}