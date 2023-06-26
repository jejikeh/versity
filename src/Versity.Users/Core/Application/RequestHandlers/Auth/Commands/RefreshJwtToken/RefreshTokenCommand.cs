using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.RefreshJwtToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthTokens>;
