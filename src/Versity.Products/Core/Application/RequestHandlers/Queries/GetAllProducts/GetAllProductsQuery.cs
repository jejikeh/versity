using Application.Abstractions.Messaging;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Queries.GetAllProducts;

public record GetAllProductsQuery(int Page) : IQuery<IEnumerable<Product>>;
