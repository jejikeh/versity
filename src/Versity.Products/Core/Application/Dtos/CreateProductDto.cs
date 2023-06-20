using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Dtos;

public record CreateProductDto(
    string Title,
    string Description,
    string Author,
    DateOnly Release);