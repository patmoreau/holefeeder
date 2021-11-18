using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries;

public class GetTransactionsHandler 
    : IRequestHandler<GetTransactionsRequestQuery, (int Total, IEnumerable<TransactionViewModel> Items)>
{
    private readonly ITransactionQueriesRepository _repository;
    private readonly ItemsCache _cache;

    public GetTransactionsHandler(ITransactionQueriesRepository repository, ItemsCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<(int Total, IEnumerable<TransactionViewModel> Items)> Handle(GetTransactionsRequestQuery requestQuery,
        CancellationToken cancellationToken)
    {
        var (totalCount, transactions) =
            (await _repository.FindAsync((Guid)_cache["UserId"], QueryParams.Create(requestQuery), cancellationToken));

        return new ValueTuple<int, IEnumerable<TransactionViewModel>>(totalCount, transactions);
    }
}
