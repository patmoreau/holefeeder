using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    public class CreateStoreItemCommandHandler : IRequestHandler<CreateStoreItemCommand, CommandResult<Guid>>
    {
        private readonly IStoreRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public CreateStoreItemCommandHandler(IStoreRepository repository, ItemsCache cache,
            ILogger<CreateStoreItemCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CommandResult<Guid>> Handle(CreateStoreItemCommand request,
            CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            if (await _repository.FindByCodeAsync((Guid)_cache["UserId"], request.Code, cancellationToken) != null)
            {
                return new CommandResult<Guid>(CommandStatus.BadRequest, Guid.Empty,
                    ImmutableArray.Create($"Code '{request.Code}' already exists."));
            }

            var storeItem = StoreItem.Create(request.Code, request.Data, (Guid)_cache["UserId"]);

            _logger.LogInformation("----- Create Store Item - StoreItem: {@StoreItem}", storeItem);

            await _repository.SaveAsync(storeItem, cancellationToken);

            await _repository.UnitOfWork.CommitAsync(cancellationToken);

            return new CommandResult<Guid>(CommandStatus.Created, storeItem.Id, ImmutableArray<string>.Empty);
        }
    }
}
