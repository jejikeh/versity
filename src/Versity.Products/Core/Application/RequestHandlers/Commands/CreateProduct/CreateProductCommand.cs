using Application.Abstractions.Messaging;
using Application.Dtos;
using Domain.Models;

namespace Application.RequestHandlers.Commands.CreateProduct;

public record CreateProductCommand(
    string Title,
    string Description,
    string Author,
    DateOnly Release) : ICommand<Product>;
