using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Sessions.Commands.CloseSession;

public record CloseSessionCommand(Guid Id) : IRequest<GetSessionByIdViewModel>;
