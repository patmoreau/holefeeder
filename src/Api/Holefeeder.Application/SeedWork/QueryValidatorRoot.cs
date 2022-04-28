using FluentValidation;

namespace Holefeeder.Application.SeedWork;

public abstract class QueryValidatorRoot<TRequest> : AbstractValidator<TRequest>
    where TRequest : IRequestQuery
{
    protected QueryValidatorRoot()
    {
        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage(x => $"'{nameof(x.Offset)}' must be >= 0.");
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithMessage(x => $"'{nameof(x.Limit)}' must be > 0.");
    }
}
