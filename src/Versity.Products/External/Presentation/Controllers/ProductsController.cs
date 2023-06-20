using Application.RequestHandlers.Commands.CreateProduct;
using Application.RequestHandlers.Commands.UpdateProduct;
using Application.RequestHandlers.Queries.GetAllProducts;
using Application.RequestHandlers.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("api/[controller]/")]
public sealed class ProductsController : ApiController
{
    public ProductsController(ISender sender) : base(sender)
    {
    }

    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetAllProducts(int page, CancellationToken cancellationToken)
    {
        var command = new GetAllProductsQuery(page);
        var result = await Sender.Send(command, cancellationToken);
        return result.IsFailed ? Problem(result.Errors.ToList()[0].Message) : Ok(result.Value);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetProductByIdQuery(id);
        var result = await Sender.Send(command, cancellationToken);
        return result.IsFailed ? Problem(result.Errors.ToList()[0].Message) : Ok(result.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductCommand createProductCommand, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(createProductCommand, cancellationToken);
        return result.IsFailed ? Problem(result.Errors.ToList()[0].Message) : Ok(result.Value);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateProduct(UpdateProductCommand updateProductCommand, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(updateProductCommand, cancellationToken);
        return result.IsFailed ? Problem(result.Errors.ToList()[0].Message) : Ok(result.Value);
    }
}