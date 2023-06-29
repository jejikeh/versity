using Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Users.Queries.GetVersityUserById;

public record GetVersityUserByIdQuery(string Id) : IRequest<ViewVersityUserDto>;