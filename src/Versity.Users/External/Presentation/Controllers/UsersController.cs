using Application.Dtos;
using Application.RequestHandlers.Users.Commands.ChangeUserPassword;
using Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;
using Application.RequestHandlers.Users.Queries.GetAllVersityUsers;
using Application.RequestHandlers.Users.Queries.GetVersityUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("api/[controller]/")]
public sealed class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender)
    {
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetVersityUserByIdQuery(id.ToString());
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{page:int}")]
    public async Task<IActionResult> GetAllUsers(int page, CancellationToken cancellationToken)
    {
        var command = new GetAllVersityUsersQuery(page);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin,Member")]
    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangeUserPassword(string id, ChangeUserPasswordDto changeUserPasswordDto,CancellationToken cancellationToken)
    {
        var command = new ChangeUserPasswordCommand(
            changeUserPasswordDto.OldPassword, 
            changeUserPasswordDto.NewPassword,
            id);
        var result = await Sender.Send(command, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("setadmin/{id:guid}")]
    public async Task<IActionResult> SetAdmin(Guid id, CancellationToken cancellationToken)
    {        
        var command = new GiveAdminRoleToUserCommand(id.ToString());
        var token  = await Sender.Send(command, cancellationToken);
        
        return Ok(new { Token = token });
    }
}