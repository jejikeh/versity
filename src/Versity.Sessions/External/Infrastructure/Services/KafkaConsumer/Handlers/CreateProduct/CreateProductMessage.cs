namespace Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;

public record CreateProductMessage(Guid Id, string Title);