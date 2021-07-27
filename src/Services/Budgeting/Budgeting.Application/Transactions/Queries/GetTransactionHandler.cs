using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries
{
    public class GetTransactionHandler : IRequestHandler<GetTransactionQuery, TransactionViewModel>
    {
        private readonly ITransactionQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetTransactionHandler(ITransactionQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<TransactionViewModel> Handle(GetTransactionQuery query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return HandleInternal(query, cancellationToken);
        }

        private async Task<TransactionViewModel> HandleInternal(GetTransactionQuery query,
            CancellationToken cancellationToken)
        {
            var transaction = (await _repository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken));

            return transaction;
        }
    }
}