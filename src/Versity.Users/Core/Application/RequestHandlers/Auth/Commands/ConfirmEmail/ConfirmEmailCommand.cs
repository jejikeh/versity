using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest<IdentityResult>;
