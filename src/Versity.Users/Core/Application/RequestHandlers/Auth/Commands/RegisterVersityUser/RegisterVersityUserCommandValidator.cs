using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.RequestHandlers.Auth.Commands.RegisterVersityUser;

public class RegisterVersityUserCommandValidator : AbstractValidator<RegisterVersityUserCommand>
{
    public RegisterVersityUserCommandValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter the correct email.");
        
        RuleFor(user => user.Phone)
            .NotEmpty().WithMessage("Phone Number is required.")
            .Matches(new Regex(@"^((8|\+374|\+994|\+995|\+375|\+7|\+380|\+38|\+996|\+998|\+993)[\- ]?)?\(?\d{3,5}\)?[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}(([\- ]?\d{1})?[\- ]?\d{1})?$")).WithMessage("PhoneNumber not valid");

        RuleFor(user => user.FirstName)
            .NotEmpty().WithMessage("First Name is required.");
        
        RuleFor(user => user.LastName)
            .NotEmpty().WithMessage("Last Name is required.");
        
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(8).WithMessage("Password length must be at least 8.")
            .MaximumLength(16).WithMessage("Password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Password must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("Password must contain at least one (!? *.).");
    }
}