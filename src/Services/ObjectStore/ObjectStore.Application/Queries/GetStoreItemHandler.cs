using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemHandler : IRequestHandler<GetStoreItemQuery, StoreItemViewModel>
    {
        private readonly IStoreItemsQueriesRepository _itemsQueriesRepository;
        private readonly ItemsCache _cache;

        public GetStoreItemHandler(IStoreItemsQueriesRepository itemsQueriesRepository, ItemsCache cache)
        {
            _itemsQueriesRepository = itemsQueriesRepository;
            _cache = cache;
        }

        public Task<StoreItemViewModel> Handle(GetStoreItemQuery query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return HandleInternal(query, cancellationToken);
        }

        private async Task<StoreItemViewModel> HandleInternal(GetStoreItemQuery query, CancellationToken cancellationToken)
        {
            return await _itemsQueriesRepository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken);
        }
    }
}
