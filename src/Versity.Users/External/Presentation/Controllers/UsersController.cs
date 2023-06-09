using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;
using Presentation.Configurations;

namespace Presentation.Controllers;

[Route("api/[controller]/")]
public sealed class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender)
    {
    }

    [VersityRoleAuthorize(VersityRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetSomething()
    {
        return Ok("Hello world, Admin!");
    }
}