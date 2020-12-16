using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemsHandler : IRequestHandler<GetStoreItemsQuery, StoreItemViewModel[]>
    {
        private readonly IStoreQueriesRepository _queriesRepository;

        public GetStoreItemsHandler(IStoreQueriesRepository queriesRepository)
        {
            _queriesRepository = queriesRepository;
        }

        public async Task<StoreItemViewModel[]> Handle(GetStoreItemsQuery query,
            CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            var results = await _queriesRepository.GetItemsAsync(query.UserId, query.Query, cancellationToken);
            return results.ToArray();
        }
    }
}
