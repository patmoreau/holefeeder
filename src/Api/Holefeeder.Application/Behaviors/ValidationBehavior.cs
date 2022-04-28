using FluentValidation;

using MediatR;

namespace Holefeeder.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest> _validator;

    public ValidationBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return await next();
    }
}
