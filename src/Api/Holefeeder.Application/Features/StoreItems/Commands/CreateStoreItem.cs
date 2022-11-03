using Carter;

using FluentValidation;

using Holefeeder.Application.Context;
using Holefeeder.Application.Domain.StoreItem;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

using StoreItem = Holefeeder.Application.Domain.StoreItem.StoreItem;

namespace Holefeeder.Application.Features.StoreItems.Commands;

public class CreateStoreItem : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/store-items/create-store-item",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetStoreItem), new {Id = result}, new {Id = result});
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(StoreItems))
            .WithName(nameof(CreateStoreItem))
            .RequireAuthorization();
    }

    internal record Request(string Code, string Data) : IRequest<Guid>, IStoreItemRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IUserContext _userContext;
        private readonly StoreItemContext _context;

        public Handler(IUserContext userContext, StoreItemContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await _context.StoreItems.AsQueryable()
                    .AnyAsync(e => e.Code == request.Code, cancellationToken: cancellationToken))
            {
                throw new ObjectStoreDomainException($"Code '{request.Code}' already exists.");
            }
            var storeItem = StoreItem.Create(request.Code, request.Data, _userContext.UserId);

            _context.StoreItems.Add(storeItem);

            return storeItem.Id;
        }
    }
}
