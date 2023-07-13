using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CloseSession;

public record CloseSessionCommand(Guid Id) : IRequest;
