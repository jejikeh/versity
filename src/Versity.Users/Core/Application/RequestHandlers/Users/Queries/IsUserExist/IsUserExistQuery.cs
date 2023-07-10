using MediatR;

namespace Application.RequestHandlers.Users.Queries.IsUserExist;

public record IsUserExistQuery(string UserId) : IRequest<bool>;