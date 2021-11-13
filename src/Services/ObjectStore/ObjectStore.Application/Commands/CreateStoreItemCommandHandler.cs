using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    public class CreateStoreItemCommandHandler : IRequestHandler<CreateStoreItemCommand, Guid>
    {
        private readonly IStoreItemsRepository _itemsRepository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public CreateStoreItemCommandHandler(IStoreItemsRepository itemsRepository, ItemsCache cache,
            ILogger<CreateStoreItemCommandHandler> logger)
        {
            _itemsRepository = itemsRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateStoreItemCommand request, CancellationToken cancellationToken)
        {
            if (await _itemsRepository.FindByCodeAsync((Guid)_cache["UserId"], request.Code, cancellationToken) != null)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.Code), $"Code '{request.Code}' already exists.")
                });
            }

            var storeItem = StoreItem.Create(request.Code, request.Data, (Guid)_cache["UserId"]);

            _logger.LogInformation("----- Create Store Item - StoreItem: {@StoreItem}", storeItem);

            await _itemsRepository.SaveAsync(storeItem, cancellationToken);

            await _itemsRepository.UnitOfWork.CommitAsync(cancellationToken);

            return storeItem.Id;
        }
    }
}
