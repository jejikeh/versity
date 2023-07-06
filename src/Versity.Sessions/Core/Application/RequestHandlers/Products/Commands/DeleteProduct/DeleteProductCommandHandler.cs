using Application.Abstractions.Repositories;
using Application.Exceptions;
using MediatR;

namespace Application.RequestHandlers.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductsRepository _productsRepository;

    public DeleteProductCommandHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productsRepository.GetProductByExternalIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no product with this External Id");
        }
        
        _productsRepository.DeleteProduct(product);
        await _productsRepository.SaveChangesAsync(cancellationToken);
    }
}