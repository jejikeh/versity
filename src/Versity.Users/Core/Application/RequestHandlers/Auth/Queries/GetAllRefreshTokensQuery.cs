using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Auth.Queries;

public record GetAllRefreshTokensQuery(int Page) : IRequest<IEnumerable<RefreshToken>>;