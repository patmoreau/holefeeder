using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

public class GetStoreItemHandler : IRequestHandler<GetStoreItemQuery, StoreItemViewModel>
{
    private readonly IStoreItemsQueriesRepository _itemsQueriesRepository;
    private readonly ItemsCache _cache;

    public GetStoreItemHandler(IStoreItemsQueriesRepository itemsQueriesRepository, ItemsCache cache)
    {
        _itemsQueriesRepository = itemsQueriesRepository;
        _cache = cache;
    }

    public async Task<StoreItemViewModel> Handle(GetStoreItemQuery query, CancellationToken cancellationToken)
    {
        var result =
            await _itemsQueriesRepository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken);
        if (result is null)
        {
            throw NotFoundDomainException.Create<StoreItem>();
        }

        return result;
    }
}
