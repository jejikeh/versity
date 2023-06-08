using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Versity.Users.Abstractions;

namespace Versity.Users.Controllers;

[Route("api/[controller]/")]
public sealed class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetSomething()
    {
        return Ok("Hello world!");
    }
}