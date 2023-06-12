using System.Security.Claims;
using Application.Dtos;
using Application.RequestHandlers.Users.Commands.ChangeUserPassword;
using Application.RequestHandlers.Users.Queries.GetAllVersityUsers;
using Application.RequestHandlers.Users.Queries.GetVersityUserById;
using Domain.Models;
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
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var command = new GetAllVersityUsersCommand();
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Member,Admin")]
    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangeUserPassword(string id, ChangeUserPasswordDto changeUserPasswordDto,CancellationToken cancellationToken)
    {
        // TODO: Or get user id from endpoint argument?
        var userId = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        if (userId == string.Empty)
            throw new Exception("Something went wrong... Empty claims");

        if (id == userId)
        {
            var command = new ChangeUserPasswordCommand(
                userId, 
                changeUserPasswordDto.OldPassword, 
                changeUserPasswordDto.NewPassword);
            var result = await Sender.Send(command, cancellationToken);
            return Ok(result);
        }
        
        return BadRequest();
    }
}