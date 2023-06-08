﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Versity.Users.Abstractions;
using Versity.Users.Core.Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using Versity.Users.Core.Application.RequestHandlers.Commands.RegisterVersityUser;
using Versity.Users.Dtos;

namespace Versity.Users.Controllers;

[Route("api/[controller]/[action]")]
public sealed class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender)
    {
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

        var result = await Sender.Send(command, cancellationToken);
        return result ? Accepted() : BadRequest();
    }
}