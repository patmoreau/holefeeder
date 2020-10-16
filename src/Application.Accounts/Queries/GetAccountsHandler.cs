using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, AccountViewModel[]>
    {
        private readonly IAccountQueriesRepository _repository;

        public GetAccountsHandler(IAccountQueriesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<AccountViewModel[]> Handle(GetAccountsQuery query,
            CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            return (await _repository.GetAccountsAsync(query.UserId, query.Query, cancellationToken)).ToArray();
        }
    }
}
