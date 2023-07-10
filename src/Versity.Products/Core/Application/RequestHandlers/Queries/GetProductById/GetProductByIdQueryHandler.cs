using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    private readonly IVersityProductsRepository _products;

    public GetProductByIdQueryHandler(IVersityProductsRepository products)
    {
        _products = products;
    }

    public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _products.GetProductByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }

        return product;
    }
}