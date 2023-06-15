using System.Security.Claims;
using Application.Dtos;
using Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("api/[controller]/[action]")]
public sealed class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVersityUserDto userDto, CancellationToken cancellationToken)
    {
        var command = new RegisterVersityUserCommand(userDto.FirstName, userDto.LastName, userDto.Email, userDto.Phone, userDto.Password);
        var result = await Sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginVersityUserDto userDto, CancellationToken cancellationToken)
    {
        var command = new LoginVersityUserCommand(userDto.Email, userDto.Password);
        var token = await Sender.Send(command, cancellationToken);
        return Ok(new { Token = token });
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> GetAdminRole(CancellationToken cancellationToken)
    {        
        var command = new GiveAdminRoleToUserCommand();
        var token  = await Sender.Send(command, cancellationToken);
        return Ok(new { Token = token });
    }
}