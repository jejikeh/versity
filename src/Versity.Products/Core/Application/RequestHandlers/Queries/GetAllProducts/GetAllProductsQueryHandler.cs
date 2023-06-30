using Application.Abstractions.Repositories;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
{
    private readonly IVersityProductsRepository _products;

    public GetAllProductsQueryHandler(IVersityProductsRepository products)
    {
        _products = products;
    }

    public Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = _products
            .GetAllProducts()
            .OrderBy(x => x.Title)
            .Skip(10 * (request.Page - 1))
            .Take(10)
            .ToList();

        return Task.Run(() => (IEnumerable<Product>)products, cancellationToken);
    }
}