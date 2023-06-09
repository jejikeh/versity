using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Versity.Users.Abstractions;
using Versity.Users.Core.Application.RequestHandlers.Auth.Commands.GetAdminRole;
using Versity.Users.Core.Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using Versity.Users.Core.Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Versity.Users.Dtos;
using Versity.Users.Infrastructure.Services.Interfaces;

namespace Versity.Users.Controllers;

[Route("api/[controller]/[action]")]
public sealed class AuthController : ApiController
{
    private readonly IAuthTokenGeneratorService _tokenGeneratorService;
    
    public AuthController(ISender sender, IAuthTokenGeneratorService tokenGeneratorService) : base(sender)
    {
        _tokenGeneratorService = tokenGeneratorService;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVersityUserDto userDto, CancellationToken cancellationToken)
    {
        var command = new RegisterVersityUserCommand(
            userDto.FirstName, userDto.LastName, userDto.Email, userDto.Phone, userDto.Password);

        var result = await Sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginVersityUserDto userDto, CancellationToken cancellationToken)
    {
        var command = new LoginVersityUserCommand(userDto.Email, userDto.Password);

        var (user, result) = await Sender.Send(command, cancellationToken);
        return result ? Ok(_tokenGeneratorService.GenerateToken(user, "Member")) : BadRequest();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> GetAdminRole(CancellationToken cancellationToken)
    {
        var userId = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        if (userId == string.Empty)
            return StatusCode(StatusCodes.Status500InternalServerError);
        
        var command = new GetAdminRoleCommand(userId);
        var user  = await Sender.Send(command, cancellationToken);
        return Ok(_tokenGeneratorService.GenerateToken(user, "Admin", "Member"));
    }
}