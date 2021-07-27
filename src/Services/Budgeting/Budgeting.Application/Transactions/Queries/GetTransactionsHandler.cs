using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries
{
    public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, QueryResult<TransactionViewModel>>
    {
        private readonly ITransactionQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetTransactionsHandler(ITransactionQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<QueryResult<TransactionViewModel>> Handle(GetTransactionsQuery query,
            CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return HandleInternal(query, cancellationToken);
        }

        private async Task<QueryResult<TransactionViewModel>> HandleInternal(GetTransactionsQuery query,
            CancellationToken cancellationToken)
        {
            var (totalCount, transactions) =
                (await _repository.FindAsync((Guid)_cache["UserId"], query.Query, cancellationToken));

            return new QueryResult<TransactionViewModel>(totalCount, transactions);
        }
    }
}
