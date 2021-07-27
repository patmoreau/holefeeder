using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries
{
    public class GetAccountHandler : IRequestHandler<GetAccountQuery, AccountViewModel>
    {
        private readonly IAccountQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetAccountHandler(IAccountQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<AccountViewModel> Handle(GetAccountQuery query,
            CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return HandleInternal(query, cancellationToken);
        }

        private async Task<AccountViewModel> HandleInternal(GetAccountQuery query, CancellationToken cancellationToken)
        {
            var results = await _repository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken);
            return results;
        }
    }
}
