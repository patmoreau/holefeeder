using FluentValidation;
using FluentValidation.Results;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public interface IValidator<in TRequest, out TResponse> : IValidator<TRequest>
{
    TResponse CreateResponse(ValidationResult result);
}
