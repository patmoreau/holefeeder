using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Filters;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.StoreItem;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.StoreItems.Commands;

public class CreateStoreItem : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/store-items/create-store-item",
                async (Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsSuccess: true } => Results.CreatedAtRoute(nameof(GetStoreItem), new { Id = (Guid)result.Value },
                            new { Id = (Guid)result.Value }),
                        _ => result.Error.ToProblem()
                    };
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .AddEndpointFilter<UnitOfWorkFilter>()
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(StoreItems))
            .WithName(nameof(CreateStoreItem))
            .RequireAuthorization(Policies.WriteUser);

    private static async Task<Result<StoreItemId>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        if (await context.StoreItems.AnyAsync(e => e.Code == request.Code, cancellationToken))
        {
            return StoreItemErrors.CodeAlreadyExists(request.Code);
        }

        var result = StoreItem.Create(request.Code, request.Data, userContext.Id);

        if (result.IsFailure)
        {
            return result.Error;
        }

        await context.StoreItems.AddAsync(result.Value, cancellationToken);

        return result.Value.Id;
    }

    public record Request(string Code, string Data);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();
        }
    }
}
