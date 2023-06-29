using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Auth.Queries;

public record GetAllRefreshTokensQuery : IRequest<IEnumerable<RefreshToken>>;