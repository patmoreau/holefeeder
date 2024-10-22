using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Authorization;
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
        app.MapGet("api/v2/store-items/{id}",
                async (StoreItemId id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new Request(id), cancellationToken);
                    return result switch
                    {
                        { IsSuccess: true } => Results.Ok(result.Value),
                        _ => result.Error.ToProblem()
                    };
                })
            .Produces<StoreItemViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(StoreItems))
            .WithName(nameof(GetStoreItem))
            .RequireAuthorization(Policies.ReadUser);

    internal record Request(StoreItemId Id) : IRequest<Result<StoreItemViewModel>>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Id).NotEqual(StoreItemId.Empty);
    }

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Result<StoreItemViewModel>>
    {
        public async Task<Result<StoreItemViewModel>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await context.StoreItems
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == userContext.Id,
                    cancellationToken);

            if (result is null)
            {
                return Result<StoreItemViewModel>.Failure(StoreItemErrors.NotFound(request.Id));
            }

            return Result<StoreItemViewModel>.Success(StoreItemMapper.MapToDto(result));
        }
    }
}
