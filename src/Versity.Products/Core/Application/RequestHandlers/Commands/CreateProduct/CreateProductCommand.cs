using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Commands.CreateProduct;

public record CreateProductCommand(
    string Title,
    string Description,
    string Author,
    DateOnly Release) : IRequest<Product>;
