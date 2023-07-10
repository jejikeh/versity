using Application.Dtos;
using Application.RequestHandlers.Commands.CreateProduct;
using Application.RequestHandlers.Commands.DeleteProduct;
using Application.RequestHandlers.Commands.UpdateProduct;
using Application.RequestHandlers.Queries.GetAllProducts;
using Application.RequestHandlers.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetProductByIdQuery(id);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProductById(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);
        await Sender.Send(command, cancellationToken);
        
        return Ok();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductCommand createProductCommand, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(createProductCommand, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto updateProductDto, CancellationToken cancellationToken)
    {
        var updateProductCommand = new UpdateProductCommand(id, updateProductDto.Title, updateProductDto.Description, updateProductDto.Author, updateProductDto.Release);
        var result = await Sender.Send(updateProductCommand, cancellationToken);
        
        return Ok(result);
    }
}