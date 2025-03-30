using DrifterApps.Seeds.Application.Mediatr;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.StoreItem;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.StoreItems.Commands;

public class ModifyStoreItem : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/store-items/modify-store-item",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return result switch
                    {
                        { IsSuccess: true } => Results.NoContent(),
                        _ => result.Error.ToProblem()
                    };
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(StoreItems))
            .WithName(nameof(ModifyStoreItem))
            .RequireAuthorization(Policies.WriteUser);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEqual(StoreItemId.Empty);
            RuleFor(x => x.Data).NotEmpty();
        }
    }

    internal record Request(StoreItemId Id, string Data) : IRequest<Result<Nothing>>, IUnitOfWorkRequest;

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Result<Nothing>>
    {
        public async Task<Result<Nothing>> Handle(Request request, CancellationToken cancellationToken)
        {
            var storeItem = await context.StoreItems
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
            if (storeItem is null)
            {
                return StoreItemErrors.NotFound(StoreItemId.Create(request.Id));
            }

            context.Update(storeItem with { Data = request.Data });

            return Nothing.Value;
        }
    }
}
