using MediatR;

namespace Versity.Users.Core.Application.RequestHandlers.Users.Queries.GetVersityUserById;

public record GetVersityUserByIdCommand(string id);