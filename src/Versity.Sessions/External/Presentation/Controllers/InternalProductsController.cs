using System.Text.Json;
using Application.Abstractions.Repositories;
using Application.RequestHandlers.Products.Queries.GetAllProducts;
using Application.RequestHandlers.Sessions.Commands.CloseSession;
using Application.RequestHandlers.Sessions.Commands.CreateSession;
using Application.RequestHandlers.Sessions.Commands.DeleteSession;
using Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;
using Infrastructure.Configurations;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;
using Infrastructure.Services.KafkaConsumer.Handlers.DeleteProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

/// <summary>
/// Theese endpoints intended use only in development environment.
/// In production it will be replacing by Kafka Message Bus
/// </summary>
[Route("api/[controller]/")]
public class InternalProductsController : ApiController
{
    private IApplicationConfiguration _applicationConfiguration;
    private readonly CreateProductMessageHandler _createProductMessageHandler;
    private readonly DeleteProductMessageHandler _deleteProductMessageHandler;
    private readonly IKafkaConsumerConfiguration _kafkaConsumerConfiguration;

    public InternalProductsController(
        ISender sender, 
        IApplicationConfiguration applicationConfiguration,
        IProductsRepository productsRepository, 
        IKafkaConsumerConfiguration configuration) : base(sender)
    {
        _applicationConfiguration = applicationConfiguration;
        _createProductMessageHandler = new CreateProductMessageHandler(productsRepository, configuration);
        _deleteProductMessageHandler = new DeleteProductMessageHandler(productsRepository, configuration);
        _kafkaConsumerConfiguration = configuration;
    }
    
    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetAllProducts(int page, CancellationToken cancellationToken)
    {
        var command = new GetAllProductsQuery(page);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductMessage message, CancellationToken cancellationToken)
    {
        if (!_applicationConfiguration.IsDevelopmentEnvironment)
        {
            return BadRequest();
        }

        await _createProductMessageHandler.Handle(
            _kafkaConsumerConfiguration.CreateProductTopic, 
            JsonSerializer.Serialize(message), 
            cancellationToken);

        return Ok();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        if (!_applicationConfiguration.IsDevelopmentEnvironment)
        {
            return BadRequest();
        }

        await _deleteProductMessageHandler.Handle(
            _kafkaConsumerConfiguration.DeleteProductTopic, 
            JsonSerializer.Serialize(new DeleteProductMessage(id)),
            cancellationToken);
        
        return Ok();
    }
}