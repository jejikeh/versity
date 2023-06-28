using FluentValidation;

namespace Application.RequestHandlers.Queries.GetProductById;

public class GetProductByIdRequestValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}