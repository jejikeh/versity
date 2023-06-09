using System.Security.Claims;
using Application.RequestHandlers.Auth.Commands.GetAdminRole;
using Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;
using Presentation.Dtos;
using Versity.Users.Core.Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Versity.Users.Dtos;

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
        var userId = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        if (userId == string.Empty)
            throw new Exception("Something went wrong... Empty claims");
        
        var command = new GetAdminRoleCommand(userId);
        var token  = await Sender.Send(command, cancellationToken);
        return Ok(new { Token = token });
    }
}