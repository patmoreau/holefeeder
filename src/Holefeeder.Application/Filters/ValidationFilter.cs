using Holefeeder.Application.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.Application.Filters;

internal class ValidationFilter<TRequest> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
        {
            return await next(context);
        }

        var validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
        if (validator is null)
        {
            return await next(context);
        }

        var cancellationToken = context.HttpContext.RequestAborted;
        var validationResult = await validator.IsValidAsync(request, cancellationToken);

        if (validationResult.IsFailure)
        {
            return validationResult.Error.ToProblem();
        }

        return await next(context);
    }
}
