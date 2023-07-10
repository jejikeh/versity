using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string? Title,
    string? Description,
    string? Author,
    DateOnly? Release) : IRequest<Product>;