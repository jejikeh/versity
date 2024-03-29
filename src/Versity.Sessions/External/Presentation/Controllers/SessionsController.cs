﻿using Application.RequestHandlers.Products.Queries.GetAllProducts;
using Application.RequestHandlers.Sessions.Commands.CloseSession;
using Application.RequestHandlers.Sessions.Commands.CreateSession;
using Application.RequestHandlers.Sessions.Commands.DeleteSession;
using Application.RequestHandlers.Sessions.Queries.GetAllProductSessions;
using Application.RequestHandlers.Sessions.Queries.GetAllSessions;
using Application.RequestHandlers.Sessions.Queries.GetSessionById;
using Application.RequestHandlers.Sessions.Queries.GetUserSessionsByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("api/[controller]/")]
public class SessionsController : ApiController
{
    public SessionsController(ISender sender) : base(sender)
    {
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetAllSessions(int page, CancellationToken cancellationToken)
    {
        var command = new GetAllSessionsQuery(page);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("products/{page:int}")]
    public async Task<IActionResult> GetAllProducts(int page, CancellationToken cancellationToken)
    {
        var command = new GetAllProductsQuery(page);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSessionById(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetSessionByIdQuery(id);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin,Member")]
    [HttpGet("users/{id:guid}/{page:int}")]
    public async Task<IActionResult> GetUserSessionsByUserId(Guid id, int page, CancellationToken cancellationToken)
    {
        var command = new GetUserSessionsByUserIdQuery(id.ToString(), page);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("products/{id:guid}/{page:int}")]
    public async Task<IActionResult> GetAllProductSessions(Guid id, int page, CancellationToken cancellationToken)
    {
        var command = new GetAllProductSessionsQuery(id, page);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionCommand d, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(d, cancellationToken);
        
        return CreatedAtAction(nameof(GetSessionById), new { id = result.Id }, result);;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSession(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSessionCommand(id);
        await Sender.Send(command, cancellationToken);
        
        return Ok();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}/close")]
    public async Task<IActionResult> CloseSession(Guid id, CancellationToken cancellationToken)
    {
        var command = new CloseSessionCommand(id);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
}