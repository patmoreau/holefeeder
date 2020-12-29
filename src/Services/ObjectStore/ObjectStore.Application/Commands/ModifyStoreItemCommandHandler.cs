using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    public class ModifyStoreItemCommandHandler : IRequestHandler<ModifyStoreItemCommand, CommandResult<Unit>>
    {
        private readonly IStoreRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public ModifyStoreItemCommandHandler(IStoreRepository repository, ItemsCache cache,
            ILogger<CreateStoreItemCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CommandResult<Unit>> Handle(ModifyStoreItemCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            var storeItem = await _repository.FindByIdAsync((Guid)_cache["UserId"], request.Id, cancellationToken);
            if (storeItem == null)
            {
                return new CommandResult<Unit>(CommandStatus.NotFound);
            }

            storeItem = storeItem.SetData(request.Data);
            
            _logger.LogInformation("----- Modify Store Item - StoreItem: {@StoreItem}", storeItem);

            await _repository.SaveAsync(storeItem, cancellationToken);

            await _repository.UnitOfWork.CommitAsync(cancellationToken);

            return new CommandResult<Unit>(CommandStatus.Ok);
        }
    }
}
