using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    public class ModifyStoreItemCommandHandler : IRequestHandler<ModifyStoreItemCommand, bool>
    {
        private readonly IStoreItemsRepository _itemsRepository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public ModifyStoreItemCommandHandler(IStoreItemsRepository itemsRepository, ItemsCache cache,
            ILogger<ModifyStoreItemCommandHandler> logger)
        {
            _itemsRepository = itemsRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> Handle(ModifyStoreItemCommand request, CancellationToken cancellationToken)
        {
            var storeItem = await _itemsRepository.FindByIdAsync((Guid)_cache["UserId"], request.Id, cancellationToken);
            if (storeItem is null)
            {
                return false;
            }

            storeItem = storeItem with { Data = request.Data };

            _logger.LogInformation("----- Modify Store Item - StoreItem: {@StoreItem}", storeItem);

            await _itemsRepository.SaveAsync(storeItem, cancellationToken);

            await _itemsRepository.UnitOfWork.CommitAsync(cancellationToken);

            return true;
        }
    }
}
