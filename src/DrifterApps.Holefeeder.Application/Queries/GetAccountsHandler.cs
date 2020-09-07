using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetAccountsHandler : IRequestHandler<GetAccountsRequest, AccountViewModel[]>
    {
        private readonly IAccountQueriesRepository _repository;

        public GetAccountsHandler(IAccountQueriesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<AccountViewModel[]> Handle(GetAccountsRequest request,
            CancellationToken cancellationToken = default)
        {
            return (await _repository.GetAccountsAsync(request.UserId, request.Query, cancellationToken)).ToArray();
        }
    }
}
