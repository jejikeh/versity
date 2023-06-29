using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.DeleteSession;

public record DeleteSessionCommand(Guid Id) : IRequest;