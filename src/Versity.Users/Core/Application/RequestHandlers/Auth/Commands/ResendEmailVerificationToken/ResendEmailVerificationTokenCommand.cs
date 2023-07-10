using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;

public record ResendEmailVerificationTokenCommand(string Email, string Password) : IRequest<IdentityResult>;
