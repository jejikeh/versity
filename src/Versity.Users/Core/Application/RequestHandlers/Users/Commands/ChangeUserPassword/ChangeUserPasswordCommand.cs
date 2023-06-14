﻿using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Users.Commands.ChangeUserPassword;

public record ChangeUserPasswordCommand(string Id, string OldPassword, string NewPassword) : IRequest<IdentityResult>;
