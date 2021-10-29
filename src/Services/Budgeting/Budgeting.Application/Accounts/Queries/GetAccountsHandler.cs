using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries
{
    public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, QueryResult<AccountViewModel>>
    {
        private readonly IAccountQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetAccountsHandler(IAccountQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<QueryResult<AccountViewModel>> Handle(GetAccountsQuery query,
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
