using FluentValidation;

namespace Application.RequestHandlers.Sessions.Queries.GetAllSessions;

public class GetAllSessionQueryValidator : AbstractValidator<GetAllSessionsQuery>
{
    public GetAllSessionQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("The page must be greater than zero.");
    }
}