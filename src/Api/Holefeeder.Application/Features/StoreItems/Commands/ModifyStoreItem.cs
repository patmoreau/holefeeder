using Carter;

using FluentValidation;

using Holefeeder.Application.Features.StoreItems.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.StoreItem;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.StoreItems.Commands;

public class ModifyStoreItem : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
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
    }

    public record Request(Guid Id, string Data) : IRequest<Unit>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IUserContext _userContext;
        private readonly IStoreItemsRepository _itemsRepository;
        private readonly ILogger _logger;

        public Handler(IUserContext userContext, IStoreItemsRepository itemsRepository, ILogger<Handler> logger)
        {
            _userContext = userContext;
            _itemsRepository = itemsRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var storeItem =
                    await _itemsRepository.FindByIdAsync(_userContext.UserId, request.Id, cancellationToken);
                if (storeItem is null)
                {
                    throw new StoreItemNotFoundException(request.Id);
                }

                storeItem = storeItem with {Data = request.Data};

                _logger.LogInformation("----- Modify Store Item - StoreItem: {@StoreItem}", storeItem);

                await _itemsRepository.SaveAsync(storeItem, cancellationToken);

                await _itemsRepository.UnitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (ObjectStoreDomainException)
            {
                _itemsRepository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
