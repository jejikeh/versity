using FluentValidation;

namespace Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public class LoginVersityUserCommandValidation : AbstractValidator<LoginVersityUserCommand>
{
    public LoginVersityUserCommandValidation()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter the correct email.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password cannot be empty");
    }
}