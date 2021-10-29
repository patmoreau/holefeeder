using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries
{
    public class GetCashflowsHandler : IRequestHandler<GetCashflowsQuery, QueryResult<CashflowViewModel>>
    {
        private readonly ICashflowQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetCashflowsHandler(ICashflowQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<QueryResult<CashflowViewModel>> Handle(GetCashflowsQuery query,
            CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return _repository.FindAsync((Guid)_cache["UserId"], query.Query, cancellationToken);
        }
    }
}
