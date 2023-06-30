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
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSessionById(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetSessionByIdQuery(id);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin,Member")]
    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUserSessionsByUserId(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetUserSessionsByUserIdQuery(id.ToString());
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetAllProductSessions(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetAllProductSessionsQuery(id);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSession(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSessionCommand(id);
        await Sender.Send(command, cancellationToken);
        
        return Ok();
    }
}