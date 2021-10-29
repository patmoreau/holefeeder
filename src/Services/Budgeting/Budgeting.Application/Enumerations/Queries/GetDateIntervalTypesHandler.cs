using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries
{
    public class GetDateIntervalTypesHandler : IRequestHandler<GetDateIntervalTypesQuery, DateIntervalType[]>
    {
        public GetDateIntervalTypesHandler()
        {
        }

        public Task<DateIntervalType[]> Handle(GetDateIntervalTypesQuery query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return Task.FromResult(Enumeration.GetAll<DateIntervalType>().ToArray());
        }
    }
}
