using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Products.Commands.CreateProduct;

public record CreateProductCommand(Guid Id, string Title) : IRequest<Product>;