using Carter;

using FluentValidation;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.StoreItem;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

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

    public record Request(string Code, string Data) : IRequest<Guid>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator(IUserContext userContext, IStoreItemsRepository itemsRepository)
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .MustAsync(async (code, cancellation) =>
                    (await itemsRepository.FindByCodeAsync(userContext.UserId, code, cancellation)) is null)
                .WithMessage(x => $"Code '{x.Code}' already exists.")
                .WithErrorCode("AlreadyExistsValidator");
            RuleFor(x => x.Data).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, Guid>
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

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var storeItem = StoreItem.Create(request.Code, request.Data, _userContext.UserId);

                _logger.LogInformation("----- Create Store Item - StoreItem: {@StoreItem}", storeItem);

                await _itemsRepository.SaveAsync(storeItem, cancellationToken);

                await _itemsRepository.UnitOfWork.CommitAsync(cancellationToken);

                return storeItem.Id;
            }
            catch (ObjectStoreDomainException)
            {
                _itemsRepository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
