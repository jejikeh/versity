using Application.Dtos;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CreateSession;

public record CreateSessionCommand(
    string UserId,
    Guid ProductId,
    DateTime Start,
    DateTime Expiry) : IRequest<SessionViewModel>;