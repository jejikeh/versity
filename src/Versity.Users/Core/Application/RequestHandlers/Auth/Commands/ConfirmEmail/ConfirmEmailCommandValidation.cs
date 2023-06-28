using FluentValidation;

namespace Application.RequestHandlers.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandValidation : AbstractValidator<ConfirmEmailCommand>  
{
    public ConfirmEmailCommandValidation()
    {
        RuleFor(request => request.UserId)
            .NotEmpty().WithMessage("User Id is required.")
            .Matches(@"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?")
            .WithMessage("The Guid ID is incorrect! Double-check your Guid ID.");

        RuleFor(request => request.Token).NotEmpty().WithMessage("Token from Email is required.");
    }
}