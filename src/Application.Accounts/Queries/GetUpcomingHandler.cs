using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUpcomingHandler : IRequestHandler<GetUpcomingQuery, UpcomingViewModel[]>
    {
        private readonly IUpcomingQueriesRepository _repository;

        public GetUpcomingHandler(IUpcomingQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<UpcomingViewModel[]> Handle(GetUpcomingQuery query,
            CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            return (await _repository.GetUpcomingAsync(query.UserId, query.From, query.To, cancellationToken))
                .ToArray();
        }
    }
}
