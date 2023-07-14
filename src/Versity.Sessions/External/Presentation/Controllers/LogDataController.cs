using Application.RequestHandlers.SessionLogging.Commands.CreateLogData;
using Application.RequestHandlers.SessionLogging.Commands.CreateLogsData;
using Application.RequestHandlers.SessionLogging.Queries.GetAllLogsData;
using Application.RequestHandlers.SessionLogging.Queries.GetLogDataById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("api/[controller]/")]
public class LogDataController : ApiController
{
    public LogDataController(ISender sender) : base(sender)
    {
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetAllLogsData(int page, CancellationToken cancellationToken)
    {
        var command = new GetAllLogsDataQuery(page);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLogDataById(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetLogDataByIdQuery(id);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateLogData([FromBody] CreateLogDataCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("{sessionLogId:guid}")]
    public async Task<IActionResult> CreateLogsData(
        Guid sessionLogId, 
        [FromBody] IEnumerable<CreateLogDataDto> createLogDataDto, 
        CancellationToken cancellationToken)
    {
        var command = new CreateLogsDataCommand(sessionLogId, createLogDataDto);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
}