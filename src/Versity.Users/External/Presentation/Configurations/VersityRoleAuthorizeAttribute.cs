using Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Configurations;

public class VersityRoleAuthorizeAttribute : AuthorizeAttribute
{
    public VersityRoleAuthorizeAttribute(VersityRole role)
    {
        Roles = role.ToString();
    }
}