using DrifterApps.Seeds.Application.Mediatr;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.StoreItems.Exceptions;
using Holefeeder.Application.UserContext;

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
                    _ = await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(StoreItems))
            .WithName(nameof(ModifyStoreItem))
            .RequireAuthorization();

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();
        }
    }

    internal record Request(Guid Id, string Data) : IRequest<Unit>, IUnitOfWorkRequest;

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Unit>
    {
        private readonly BudgetingContext _context = context;
        private readonly IUserContext _userContext = userContext;

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var storeItem = await _context.StoreItems
                // .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == _userContext.Id, cancellationToken);
            if (storeItem is null)
            {
                throw new StoreItemNotFoundException(request.Id);
            }

            _context.Update(storeItem with { Data = request.Data });

            return Unit.Value;
        }
    }
}
