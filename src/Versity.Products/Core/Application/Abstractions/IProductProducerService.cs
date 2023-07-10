using Application.Dtos;
using Application.RequestHandlers.Commands.CreateProduct;

namespace Application.Abstractions;

public interface IProductProducerService
{
    public Task CreateProductProduce(CreateProductProduceDto createProductProduceDto, CancellationToken cancellationToken);
    public Task DeleteProductProduce(DeleteProductDto id, CancellationToken cancellationToken);
}