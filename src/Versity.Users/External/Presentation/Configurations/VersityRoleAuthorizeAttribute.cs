﻿using Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Configurations;

public class VersityRoleAuthorizeAttribute : AuthorizeAttribute
{
    public VersityRoleAuthorizeAttribute(VersityRoles role) : base(policy: role.ToString())
    {
    }
}