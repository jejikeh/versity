﻿using System.Text;
using Application.Dtos;
using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using Application.RequestHandlers.Auth.Commands.LoginVersityUser;
using Application.RequestHandlers.Auth.Commands.RefreshJwtToken;
using Application.RequestHandlers.Auth.Commands.RegisterVersityUser;
using Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;
using Application.RequestHandlers.Auth.Queries;
using Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
        
        return result.Succeeded ? Ok("The confirmation message was send to your email!") : BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVersityUserDto userDto, CancellationToken cancellationToken)
    {
        var command = new LoginVersityUserCommand(userDto.Email, userDto.Password);
        var token = await Sender.Send(command, cancellationToken);

        return Ok(token);
    }

    [HttpGet("{userId}/{code}")]
    public async Task<IActionResult> ConfirmEmail(string userId, string code, CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(userId, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)));
        var token  = await Sender.Send(command, cancellationToken);
        
        return Ok(token.Succeeded ? "Thank you for confirming your mail." : "Your Email is not confirmed");
    }
    
    [HttpPost]
    public async Task<IActionResult> ResendEmailVerificationToken(LoginVersityUserDto userDto, CancellationToken cancellationToken)
    {
        var command = new ResendEmailVerificationTokenCommand(userDto.Email, userDto.Password);
        var token = await Sender.Send(command, cancellationToken);
        
        return Ok(token.Succeeded ? "Email verification token was send" : "Email verification token didnt send");
    }
    
    [HttpPost("{userId}/{refreshToken}")]
    public async Task<IActionResult> RefreshToken(string userId, string refreshToken, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(userId, refreshToken);
        var token = await Sender.Send(command, cancellationToken);
        
        return Ok(token);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllRefreshTokens(CancellationToken cancellationToken)
    {
        var command = new GetAllRefreshTokensQuery();
        var token = await Sender.Send(command, cancellationToken);
        
        return Ok(token);
    }
}