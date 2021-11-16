using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

public static class GetStoreItem
{
    public record Request : IRequest<IRequestResult>, IRequestById
    {
        public Guid Id { get; init; }
    }
    
    public class Handler : IRequestHandler<Request, IRequestResult>
    {
        private readonly IStoreItemsQueriesRepository _itemsQueriesRepository;
        private readonly ItemsCache _cache;

        public Handler(IStoreItemsQueriesRepository itemsQueriesRepository, ItemsCache cache)
        {
            _itemsQueriesRepository = itemsQueriesRepository;
            _cache = cache;
        }

        public async Task<IRequestResult> Handle(Request request, CancellationToken cancellationToken)
        {
            var result =
                await _itemsQueriesRepository.FindByIdAsync((Guid)_cache["UserId"], request.Id, cancellationToken);
            if (result is null)
            {
                return new NotFoundRequestResult();
            }

            return new IdRequestResult(result);
        }
    }

}
