using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.RefreshJwtToken;

public record RefreshTokenCommand(string UserId, string RefreshToken) : IRequest<AuthTokens>;
