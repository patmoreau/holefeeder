using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using OneOf;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

public static class GetStoreItem
{
    public record Request : IRequest<OneOf<StoreItemViewModel, NotFoundRequestResult>>
    {
        public Guid Id { get; init; }
    }

    public class Handler : IRequestHandler<Request, OneOf<StoreItemViewModel, NotFoundRequestResult>>
    {
        private readonly ItemsCache _cache;
        private readonly IStoreItemsQueriesRepository _itemsQueriesRepository;

        public Handler(IStoreItemsQueriesRepository itemsQueriesRepository, ItemsCache cache)
        {
            _itemsQueriesRepository = itemsQueriesRepository;
            _cache = cache;
        }

        public async Task<OneOf<StoreItemViewModel, NotFoundRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var result =
                await _itemsQueriesRepository.FindByIdAsync((Guid)_cache["UserId"], request.Id, cancellationToken);
            if (result is null)
            {
                return new NotFoundRequestResult();
            }

            return result;
        }
    }
}
