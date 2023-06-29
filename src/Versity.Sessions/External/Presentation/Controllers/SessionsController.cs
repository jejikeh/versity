using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]/")]
public class SessionController : ApiController
{
    public SessionController(ISender sender) : base(sender)
    {
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSessionById(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetVersityUserByIdCommand(id.ToString());
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
}