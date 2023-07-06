using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.RequestHandlers.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
{
    private readonly IVersityProductsRepository _products;

    public GetAllProductsQueryHandler(IVersityProductsRepository products)
    {
        _products = products;
    }

    public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _products
            .GetAllProducts()
            .OrderBy(x => x.Title)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage)
            .ToListAsync(cancellationToken);

        return products;
    }
}