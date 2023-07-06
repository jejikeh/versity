using MediatR;

namespace Application.RequestHandlers.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;
