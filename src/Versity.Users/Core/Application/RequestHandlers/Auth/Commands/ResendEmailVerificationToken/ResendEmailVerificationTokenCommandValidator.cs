using Application.RequestHandlers.Auth.Commands.ConfirmEmail;
using FluentValidation;

namespace Application.RequestHandlers.Auth.Commands.ResendEmailVerificationToken;

public class ResendEmailVerificationTokenCommandValidator : AbstractValidator<ResendEmailVerificationTokenCommand>  
{
    public ResendEmailVerificationTokenCommandValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter the correct email.");

        RuleFor(user => user.Password).NotEmpty().WithMessage("Password cannot be empty");
    }
}