using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CreateSession;

public record CreateSessionCommand(string UserId, Guid ProductId, DateTime Start, DateTime Expiry) : IRequest<SessionViewModel>;