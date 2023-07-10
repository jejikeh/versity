using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using MediatR;

namespace Application.RequestHandlers.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IVersityProductsRepository _products;
    private readonly IProductProducerService _productProducerService;

    public DeleteProductCommandHandler(IVersityProductsRepository products, IProductProducerService productProducerService)
    {
        _products = products;
        _productProducerService = productProducerService;
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
        await _productProducerService.DeleteProductProduce(new DeleteProductDto(request.Id), cancellationToken);
    }
}