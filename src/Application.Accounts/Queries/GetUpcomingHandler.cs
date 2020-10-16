using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUpcomingHandler : IRequestHandler<GetUpcomingQuery, UpcomingViewModel[]>
    {
        private readonly IUpcomingQueriesRepository _repository;

        public GetUpcomingHandler(IUpcomingQueriesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<UpcomingViewModel[]> Handle(GetUpcomingQuery query,
            CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            return (await _repository.GetUpcomingAsync(query.UserId, query.From, query.To, cancellationToken))
                .ToArray();
        }
    }
}
