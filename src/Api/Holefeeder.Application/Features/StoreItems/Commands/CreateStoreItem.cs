using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.StoreItems.Queries;
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
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetStoreItem), new { Id = result }, new { Id = result });
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(StoreItems))
            .WithName(nameof(CreateStoreItem))
            .RequireAuthorization();

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();
        }
    }

    internal record Request(string Code, string Data) : IRequest<Guid>, IUnitOfWorkRequest;

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Guid>
    {
        private readonly BudgetingContext _context = context;
        private readonly IUserContext _userContext = userContext;

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await _context.StoreItems
                    .AnyAsync(e => e.Code == request.Code, cancellationToken))
            {
                throw new ObjectStoreDomainException($"Code '{request.Code}' already exists.");
            }

            var storeItem = StoreItem.Create(request.Code, request.Data, _userContext.Id);

            _context.StoreItems.Add(storeItem);

            return storeItem.Id;
        }
    }
}
