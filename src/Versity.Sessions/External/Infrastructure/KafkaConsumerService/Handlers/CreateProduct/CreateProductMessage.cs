using Domain.Models;
using MediatR;

namespace Infrastructure.KafkaConsumerService.Handlers.CreateProduct;

public record CreateProductMessage(Guid Id, string Title);