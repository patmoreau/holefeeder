using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Domain.Enumerations;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetAccountHandler : IRequestHandler<GetAccountQuery, AccountViewModel>
    {
        private readonly IAccountQueriesRepository _repository;

        public GetAccountHandler(IAccountQueriesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<AccountViewModel> Handle(GetAccountQuery query,
            CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            return await Task.FromResult(new AccountViewModel(
                query.Id,
                AccountType.Checking,
                "Test Account",
                99, 123.45m, DateTime.Today, "This is a test account", true));
        }
    }
}
