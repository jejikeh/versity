using FluentValidation;

namespace Application.RequestHandlers.Sessions.Queries.GetAllSessions;

public class GetAllSessionRequestValidator : AbstractValidator<GetAllSessionsQuery>
{
    public GetAllSessionRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("The page must be greater than zero.");
    }
}