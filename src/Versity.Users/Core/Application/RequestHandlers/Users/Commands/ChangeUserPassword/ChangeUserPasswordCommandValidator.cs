using FluentValidation;

namespace Application.RequestHandlers.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(request => request.OldPassword).NotEmpty().WithMessage("Old Password cannot be empty");
        
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