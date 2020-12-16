using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemHandler : IRequestHandler<GetStoreItemQuery, StoreItemViewModel>
    {
        private readonly IStoreQueriesRepository _queriesRepository;

        public GetStoreItemHandler(IStoreQueriesRepository queriesRepository)
        {
            _queriesRepository = queriesRepository;
        }

        public async Task<StoreItemViewModel> Handle(GetStoreItemQuery query, CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            return await _queriesRepository.GetItemAsync(query.UserId, query.Id, cancellationToken);
        }
    }
}
