using Domain.Models;
using FluentValidation;

namespace Application.RequestHandlers.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(product => product.Title)
            .NotEmpty().WithMessage("Title is required.");
        
        RuleFor(product => product.Author)
            .NotEmpty().WithMessage("Author is required.");

        RuleFor(product => product.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(30).WithMessage("Please add more description to the description");

        RuleFor(product => product.Release)
            .NotEmpty().WithMessage("Release date is required.");
    }
}