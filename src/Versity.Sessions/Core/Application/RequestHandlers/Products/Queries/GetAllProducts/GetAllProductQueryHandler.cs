using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;
using MediatR;

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
        var products = _productsRepository
            .GetAllProducts()
            .OrderBy(x => x.Id)
            .Skip(PageFetchSettings.ItemsOnPage * (request.Page - 1))
            .Take(PageFetchSettings.ItemsOnPage);

        return await _productsRepository.ToListAsync(products);
    }
}