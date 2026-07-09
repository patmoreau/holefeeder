using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.StoreItem;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.StoreItems.Queries;

public class GetStoreItem : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/store-items/{id:guid}",
                async (Guid id, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(new Request(StoreItemId.Create(id)), userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsSuccess: true } => Results.Ok(result.Value),
                        _ => result.Error.ToProblem()
                    };
                })
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .Produces<StoreItemViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(StoreItems))
            .WithName(nameof(GetStoreItem))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<Result<StoreItemViewModel>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var result = await context.StoreItems
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == userContext.Id,
                cancellationToken);

        if (result is null)
        {
            return StoreItemErrors.NotFound(request.Id);
        }

        return StoreItemMapper.MapToDto(result);
    }

    public record Request(StoreItemId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(x => StoreItemId.Create(x)).NotEqual(StoreItemId.Empty);
    }
}
