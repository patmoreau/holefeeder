using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemsHandler : IRequestHandler<GetStoreItemsQuery, QueryResult<StoreItemViewModel>>
    {
        private readonly IStoreItemsQueriesRepository _itemsQueriesRepository;
        private readonly ItemsCache _cache;

        public GetStoreItemsHandler(IStoreItemsQueriesRepository itemsQueriesRepository, ItemsCache cache)
        {
            _itemsQueriesRepository = itemsQueriesRepository;
            _cache = cache;
        }

        public Task<QueryResult<StoreItemViewModel>> Handle(GetStoreItemsQuery query,
            CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return _itemsQueriesRepository.FindAsync((Guid)_cache["UserId"], query.Query, cancellationToken);
        }
    }
}
