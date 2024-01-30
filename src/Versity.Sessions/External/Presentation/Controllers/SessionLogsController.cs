using Application.RequestHandlers.SessionLogging.Queries.GetAllSessionsLogs;
using Application.RequestHandlers.SessionLogging.Queries.GetSessionLogsById;
using Application.RequestHandlers.Sessions.Queries.GetAllSessions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("api/[controller]/")]
public class SessionLogsController : ApiController
{
    public SessionLogsController(ISender sender) : base(sender)
    {
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetAllSessionLogs(int page, CancellationToken cancellationToken)
    {
        var command = new GetAllSessionsLogsQuery(page);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSessionLogsById(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetSessionLogsByIdQuery(id);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
}