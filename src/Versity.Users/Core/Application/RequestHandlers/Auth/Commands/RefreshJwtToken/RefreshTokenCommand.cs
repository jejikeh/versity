using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.RefreshRefreshToken;

public record RefreshTokenCommand(string UserId, string RefreshToken) : IRequest<AuthTokens>;
