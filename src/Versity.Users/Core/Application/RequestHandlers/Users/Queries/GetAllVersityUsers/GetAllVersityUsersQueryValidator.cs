using FluentValidation;

namespace Application.RequestHandlers.Users.Queries.GetAllVersityUsers;

public class GetAllVersityUsersQueryValidator : AbstractValidator<GetAllVersityUsersQuery>
{
    public GetAllVersityUsersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("The page must be greater than zero.");
    }
}