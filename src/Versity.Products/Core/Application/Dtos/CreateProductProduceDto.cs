using Domain.Models;

namespace Application.Dtos;

public record CreateProductProduceDto(Guid Id, string Title)
{
    public static CreateProductProduceDto CreateFromModel(Product product)
    {
        return new CreateProductProduceDto(product.Id, product.Title);
    }
}
