using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.RequestHandlers.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string? Title,
    string? Description,
    string? Author,
    DateOnly? Release) : ICommand<Product>;