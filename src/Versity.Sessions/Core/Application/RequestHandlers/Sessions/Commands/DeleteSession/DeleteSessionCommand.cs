using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.DeleteSession;

public record DeleteProductCommand(Guid Id) : IRequest;