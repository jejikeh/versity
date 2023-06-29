using Application.RequestHandlers.Sessions.Queries.GetSessionById;
using Application.RequestHandlers.Sessions.Queries.GetUserSessionsByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]/")]
public class SessionsController : ApiController
{
    public SessionsController(ISender sender) : base(sender)
    {
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
    [HttpGet("user/{id:guid}")]
    public async Task<IActionResult> GetUserSessionsByUserId(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetUserSessionsByUserIdQuery(id.ToString());
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
}