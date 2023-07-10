using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(int Page) : IRequest<IEnumerable<Product>>;