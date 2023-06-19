using FluentResults;
using FluentValidation;
using MediatR;

namespace Application.Behaviors;

public class ValidationPipelineBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes> 
    where TReq: IRequest<TRes>
    where TRes : ResultBase, new()
{
    private readonly IEnumerable<IValidator<TReq>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TReq>> validators)
    {
        _validators = validators;
    }

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TReq>(request);
        foreach (var validator in _validators)
        {
            var validationResult = await validator.ValidateAsync(context, cancellationToken);
            if (!validationResult.IsValid)
            {
                var result = new TRes();
                result.Reasons.AddRange(validationResult.ToResult().Reasons);
                return result;
            }
        }

        return await next();
    }
}