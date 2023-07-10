using MediatR;

namespace Application.RequestHandlers.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;
