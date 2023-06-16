using Application.Dtos;
using Application.RequestHandlers.Users.Commands.ChangeUserPassword;
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
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken)
    {
        var command = new GetVersityUserByIdCommand(id);
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetAllUsers(int page, CancellationToken cancellationToken)
    {
        var command = new GetAllVersityUsersCommand(page);
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Member")]
    [HttpPut("me/password")]
    public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordDto changeUserPasswordDto,CancellationToken cancellationToken)
    {
        var command = new ChangeUserPasswordCommand(
            changeUserPasswordDto.OldPassword, 
            changeUserPasswordDto.NewPassword);
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
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
}