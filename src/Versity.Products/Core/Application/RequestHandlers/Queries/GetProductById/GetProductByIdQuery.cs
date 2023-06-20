using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.RequestHandlers.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<Product>;
