using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, AccountViewModel[]>
    {
        private readonly IAccountQueriesRepository _repository;

        public GetAccountsHandler(IAccountQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountViewModel[]> Handle(GetAccountsQuery query,
            CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            return (await _repository.FindAsync(query.UserId, query.Query, cancellationToken)).ToArray();
        }
    }
}
