using Application.Abstractions.Repositories;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.RequestHandlers.Products.Queries.GetAllProducts;

public class GetAllProductQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
{
    private readonly IProductsRepository _productsRepository;

    public GetAllProductQueryHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productsRepository
            .GetAllProducts()
            .OrderBy(x => x.Id)
            .Skip(10 * (request.Page - 1))
            .Take(10)
            .ToListAsync(cancellationToken);
        
        return products;
    }
}