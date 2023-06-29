using FluentValidation;

namespace Application.RequestHandlers.Queries.GetAllProducts;

public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
{
    public GetAllProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("The page must be greater than zero.");
    }
}