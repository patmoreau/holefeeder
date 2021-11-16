using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Commands;

public static class ModifyStoreItem
{
    public record Request(Guid Id, string Data) : IRequest<IRequestResult>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator(ILogger<Validator> logger)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }

    public class Handler : IRequestHandler<Request, IRequestResult>
    {
        private readonly IStoreItemsRepository _itemsRepository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public Handler(IStoreItemsRepository itemsRepository, ItemsCache cache, ILogger<Handler> logger)
        {
            _itemsRepository = itemsRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IRequestResult> Handle(Request request, CancellationToken cancellationToken)
        {
            var storeItem = await _itemsRepository.FindByIdAsync((Guid)_cache["UserId"], request.Id, cancellationToken);
            if (storeItem is null)
            {
                return new NotFoundRequestResult();
            }

            storeItem = storeItem with { Data = request.Data };

            _logger.LogInformation("----- Modify Store Item - StoreItem: {@StoreItem}", storeItem);

            await _itemsRepository.SaveAsync(storeItem, cancellationToken);

            await _itemsRepository.UnitOfWork.CommitAsync(cancellationToken);

            return new NoContentResult();
        }
    }
}
