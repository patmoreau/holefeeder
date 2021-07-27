using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries
{
    public class GetCashflowHandler : IRequestHandler<GetCashflowQuery, CashflowViewModel>
    {
        private readonly ICashflowQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetCashflowHandler(ICashflowQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<CashflowViewModel> Handle(GetCashflowQuery query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return HandleInternal(query, cancellationToken);
        }

        private async Task<CashflowViewModel> HandleInternal(GetCashflowQuery query,
            CancellationToken cancellationToken)
        {
            var result = (await _repository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken));

            return result;
        }
    }
}
