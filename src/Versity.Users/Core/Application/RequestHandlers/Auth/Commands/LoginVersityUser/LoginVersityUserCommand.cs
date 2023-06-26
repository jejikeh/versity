using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public record LoginVersityUserCommand(string Email, string Password) : IRequest<AuthTokens>;