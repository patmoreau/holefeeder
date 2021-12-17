using FluentValidation;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public abstract class QueryValidatorRoot<TRequest> : AbstractValidator<TRequest>
    where TRequest : IRequestQuery, IValidateable
{
    protected QueryValidatorRoot()
    {
        RuleFor(x => x.Offset).GreaterThanOrEqualTo(0).WithMessage("offset_invalid");
        RuleFor(x => x.Limit).GreaterThan(0).WithMessage("limit_invalid");
    }
}
