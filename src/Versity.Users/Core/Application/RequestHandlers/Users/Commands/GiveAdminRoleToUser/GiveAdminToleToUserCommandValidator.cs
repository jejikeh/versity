using FluentValidation;

namespace Application.RequestHandlers.Users.Commands.GiveAdminRoleToUser;

public class GiveAdminToleToUserCommandValidator : AbstractValidator<GiveAdminRoleToUserCommand>
{
    public GiveAdminToleToUserCommandValidator()
    {
        RuleFor(user => user.UserId)
            .NotEmpty().WithMessage("User Id is required.");
    }
}