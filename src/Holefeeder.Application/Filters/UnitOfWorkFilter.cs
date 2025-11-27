using DrifterApps.Seeds.Domain;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.Application.Filters;

internal class UnitOfWorkFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var cancellationToken = context.HttpContext.RequestAborted;
        var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        await unitOfWork.BeginWorkAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var response = await next(context).ConfigureAwait(false);

            await unitOfWork.CommitWorkAsync(cancellationToken).ConfigureAwait(false);

            return response;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackWorkAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }
}
