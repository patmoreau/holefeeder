using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries
{
    public class GetUpcomingHandler : IRequestHandler<GetUpcomingQuery, UpcomingViewModel[]>
    {
        private readonly IUpcomingQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetUpcomingHandler(IUpcomingQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<UpcomingViewModel[]> Handle(GetUpcomingQuery query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return HandlerInternal(query, cancellationToken);
        }

        private async Task<UpcomingViewModel[]> HandlerInternal(GetUpcomingQuery query,
            CancellationToken cancellationToken)
        {
            var results =
                await _repository.GetUpcomingAsync((Guid)_cache["UserId"], query.From, query.To, cancellationToken);
            return results.ToArray();
        }
    }
}
