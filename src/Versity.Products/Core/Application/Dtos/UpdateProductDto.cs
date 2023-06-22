using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Dtos;

public record UpdateProductDto(
    string Title,
    string Description,
    string Author,
    DateOnly Release);