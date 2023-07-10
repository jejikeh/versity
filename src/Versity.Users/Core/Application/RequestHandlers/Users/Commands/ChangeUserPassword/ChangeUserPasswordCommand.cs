using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Users.Commands.ChangeUserPassword;

public record ChangeUserPasswordCommand(string OldPassword, string NewPassword, string Id) : IRequest<IdentityResult>;
