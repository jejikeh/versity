using Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.RequestHandlers.Users.Queries.GetVersityUserById;

public record GetVersityUserByIdCommand(string Id) : IRequest<ViewVersityUserDto>;