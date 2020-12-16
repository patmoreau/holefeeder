using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
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

            var results = await _repository.GetUpcomingAsync(query.UserId, query.From, query.To, cancellationToken);
            return results.ToArray();
        }
    }
}
