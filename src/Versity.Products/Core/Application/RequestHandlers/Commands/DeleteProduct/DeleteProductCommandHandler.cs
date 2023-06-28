using Application.Abstractions.Repositories;
using Application.Exceptions;
using MediatR;

namespace Application.RequestHandlers.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IVersityProductsRepository _products;

    public DeleteProductCommandHandler(IVersityProductsRepository products)
    {
        _products = products;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _products.GetProductByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }
        _products.DeleteProduct(product);
        await _products.SaveChangesAsync(cancellationToken);
    }
}