using FluentValidation;

namespace Application.RequestHandlers.Sessions.Commands.CreateSession;

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(request => request.UserId)
            .NotEmpty().WithMessage("User Id is required.")
            .Matches(@"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?")
            .WithMessage("The Guid ID is incorrect! Double-check User Id Guid");

        RuleFor(request => request.ProductId)
            .NotEmpty().WithMessage("Product Id is required.");

        RuleFor(request => request.Start)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Start time must be greater or equal UTC time");

        RuleFor(request => request.Expiry)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.Start).WithMessage("Expiry time must be greater than start time!");
    }
}