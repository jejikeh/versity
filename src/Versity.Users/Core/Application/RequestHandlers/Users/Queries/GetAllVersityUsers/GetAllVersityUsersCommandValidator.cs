using FluentValidation;

namespace Application.RequestHandlers.Users.Queries.GetAllVersityUsers;

public class GetAllVersityUsersCommandValidator : AbstractValidator<GetAllVersityUsersCommand>
{
    public GetAllVersityUsersCommandValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("The page must be greater than zero.");
    }
}