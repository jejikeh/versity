using FluentValidation;

namespace Application.RequestHandlers.Queries.GetAllProducts;

public class GetAllProductsRequestValidator : AbstractValidator<GetAllProductsQuery>
{
    public GetAllProductsRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("The page must be greater than zero.");
    }
}