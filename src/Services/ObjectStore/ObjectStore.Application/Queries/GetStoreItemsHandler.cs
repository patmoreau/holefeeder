using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemsHandler
        : IRequestHandler<GetStoreItemsQuery, (int Total, IEnumerable<StoreItemViewModel> Items)>
    {
        private readonly IStoreItemsQueriesRepository _itemsQueriesRepository;
        private readonly ItemsCache _cache;

        public GetStoreItemsHandler(IStoreItemsQueriesRepository itemsQueriesRepository, ItemsCache cache)
        {
            _itemsQueriesRepository = itemsQueriesRepository;
            _cache = cache;
        }

        public async Task<(int Total, IEnumerable<StoreItemViewModel> Items)> Handle(GetStoreItemsQuery query,
            CancellationToken cancellationToken)
        {
            var (total, items) =
                await _itemsQueriesRepository.FindAsync((Guid)_cache["UserId"], QueryParams.Create(query),
                    cancellationToken);

            return (total, items);
        }
    }
}
