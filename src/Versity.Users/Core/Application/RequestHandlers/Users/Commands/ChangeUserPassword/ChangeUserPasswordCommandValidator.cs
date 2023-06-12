using FluentValidation;

namespace Application.RequestHandlers.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty().WithMessage("User Id is required.")
            .Matches(@"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?")
            .WithMessage("The Guid ID is incorrect! Double-check your Guid ID.");
        
        RuleFor(request => request.OldPassword)
            .NotEmpty().WithMessage("Old Password cannot be empty");
        
        RuleFor(request => request.NewPassword)
            .NotEmpty().WithMessage("New Password cannot be empty")
            .MinimumLength(8).WithMessage("New Password length must be at least 8.")
            .MaximumLength(16).WithMessage("New Password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("New Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("New Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("New Password must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("New Password must contain at least one (!? *.).");
    }
}