using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<Product>;
