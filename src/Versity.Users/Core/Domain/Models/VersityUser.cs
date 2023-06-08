using Microsoft.AspNetCore.Identity;

namespace Versity.Users.Core.Domain.Models;

public class VersityUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}