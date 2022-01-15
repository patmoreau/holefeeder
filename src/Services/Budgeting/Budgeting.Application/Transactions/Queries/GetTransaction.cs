using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries;

public static class GetTransaction
{
    public record Request(Guid Id) : IRequest<OneOf<TransactionInfoViewModel, NotFoundRequestResult>>;

    public class Handler : IRequestHandler<Request, OneOf<TransactionInfoViewModel, NotFoundRequestResult>>
    {
        private readonly ITransactionQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public Handler(ITransactionQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<OneOf<TransactionInfoViewModel, NotFoundRequestResult>> Handle(Request query, CancellationToken cancellationToken)
        {
            var transaction = (await _repository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken));
            if (transaction is null)
            {
                return new NotFoundRequestResult();
            }

            return transaction;
        }
    }
}
