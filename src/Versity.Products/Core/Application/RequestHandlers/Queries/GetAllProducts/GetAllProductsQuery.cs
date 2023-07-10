using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Queries.GetAllProducts;

public record GetAllProductsQuery(int Page) : IRequest<IEnumerable<Product>>;
